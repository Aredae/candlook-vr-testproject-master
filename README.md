# Exploratory Project

Explores possibilities of using the Varjo VR-2 Pro for eye tracking in VR.

## Knowledge Base

Common issues and workarounds.

### Setting up a new Unity Project

Create a new 3D unity project. Then in `Window -> Package Manger`, click the `+` icon at the top left, choose `Add package from git ULR`, and use `https://github.com/varjocom/VarjoUnityXRPlugin.git` as the URL.
Then go to `Edit -> Project Settings -> XR Plug-in Management` and check `Varjo`.

When creating a new Scene, always remember to convert the main camera into an XR Rig by clicking `Game Object -> XR -> Convert Main Camera to XR Rig`.

### Camera position: What are XRRig, CameraOffset, and Main Camera?

The `XRRig` object acts as a transform that translates "real world" coordinates to Unity coordinates. When setting up the headset, the user configues a centre of the room, i.e. defines coordinates (0, 0) in the real world. These map to the `XRRig` coordinates, thus **to define where you want the user to start out, move the `XRRig` game object**.

The `XRRig` does *not* move as the user moves around in the real world. Instead, the transform of the main camera continuously is updated based in the real-world position of the headset (relative to the user-defined initial coordinates).

The `Camera Offset` transform is automatically set to zero when the game starts, assuming the room-centre and floor coordinates are porperly calibrated. If the headset is however calibrated in "stationary mode", then the camera offset is used to obtain the y offset in unity coordinates.

### Timewarp Artifacts

The headset uses accelerometers and some image processing software to warp the rendered image between frames. This ensures that head movement looks smooth, but can cause some artifacts:
- If the user spins around quickly, they will notice black bars in the direction of movement where the scene has not yet been rendered.
- Any objects that are supposed to move together with the headset will appear to "jitter" or "lag".

The former is sadly not really fixable with Unity right now. The latter problem can be worked around by disabling timewarp. This can be done either project-wide or in code for each game. For the project-wide setting, go to `Edit -> Project Settings -> XR Plug-in Management -> Varjo` and check `Face-locked`. To do it in code, run
```csharp
Varjo.XR.VarjoRendering.SetFaceLocked(true);
```
at the start of the game.

### Hand Tracking

The VR-2 Pro has external-facing infrared cameras for hand tracking, built by LeapMotion. The official Varjo LeapMotion unity packages don't work on recent versions of Unity. Instead, use the latest unity SDK from LeapMotion. As of version `4.8.0`, the shaders used for rendering hands do not work with instanced GPU rendering (learn more [here](https://docs.unity3d.com/Manual/GPUInstancing.html)), meaning that the hand will only appear on one eye but not the other (unless the render mode is changed to multi-pass, which sacrifices performance significantly). This can be easily fixed as follows:
1. Open `Assets\Plugins\LeapMotion\Modules\Hands\Models\GenericHand\GenericHandShader.shader`
2. At the end of `struct v2f`, add
```cgi
UNITY_VERTEX_OUTPUT_STEREO
```
3. In *both* vertex shaders, modify the declaration `v2f o;` to become
```cgi
v2f o;
UNITY_SETUP_INSTANCE_ID(v);
UNITY_INITIALIZE_OUTPUT(v2f, o);
UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
```

In other words, apply the following patch:
```patch
diff --git a/Assets/Plugins/LeapMotion/Modules/Hands/Models/GenericHand/GenericHandShader.shader b/Assets/Plugins/LeapMotion/Modules/Hands/Models/GenericHand/GenericHandShader.shader
index 3f4e35331..73495e9b0 100644
--- a/Assets/Plugins/LeapMotion/Modules/Hands/Models/GenericHand/GenericHandShader.shader
+++ b/Assets/Plugins/LeapMotion/Modules/Hands/Models/GenericHand/GenericHandShader.shader
@@ -38,6 +38,8 @@
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                fixed4 diff : COLOR0;
+
+               UNITY_VERTEX_OUTPUT_STEREO
        };

        ENDCG
@@ -64,6 +66,10 @@
                        v2f vert(appdata_base v)
                        {
                                v2f o;
+                               UNITY_SETUP_INSTANCE_ID(v);
+                               UNITY_INITIALIZE_OUTPUT(v2f, o);
+                               UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
+
                                o.pos = UnityObjectToClipPos(v.vertex);
                                float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                                float2 offset = TransformViewToProjection(norm.xy);
@@ -93,6 +99,10 @@
                        v2f vert(appdata_base v)
                        {
                                v2f o;
+                               UNITY_SETUP_INSTANCE_ID(v);
+                               UNITY_INITIALIZE_OUTPUT(v2f, o);
+                               UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
+
                                o.pos = UnityObjectToClipPos(v.vertex);
                                o.uv = v.texcoord;
                                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
```

### Using Hand Tracking in a new Scene

See the official documentation [here](https://leapmotion.github.io/UnityModules/index.html) but make the necessary adaptations to work with the XRRig. In summary,
- Add the `Leap XR Service Provider` component to the main camera
  - Set `Tracking Optimization` to `HMD`
  - In `Advanced Options`, set `Device Offset Mode` to `Manual` and configure
    ```
    x offset: -0.025734
    z offset:  0.068423
    z tilt:    5
    ```
    as per [Varjo's Instructions](https://developer.varjo.com/docs/get-started/hand-tracking).
- From `Assets\Plugins\LeapMotion\Modules\Hands\Prefabs`, drag your desired prefab into the scene, as a descendant of `Camera Offset` (i.e. as a sibling to the main camera).
- Add the `Hand Model Manager` component to the prefab you just added.
  - Set the `Leap Provider` to the main camera
  - In `Model Pool`, click `+`, give any name, and then drag the two children of the prefab (left and right hand) into `Left Model` and `Right Model` respectively.
  - Check `Is Enabled`
- Now you can see your hands when playing the game, but interactivity is not yet configured.
  - To do so, create an empty game object as a descendant of `Camera Offset` (right-click `Camera Offset`, then `Create Empty`.), and call it e.g. `InteractionManager`
  - Add the `Interaction Manager` component
  - Create two empty child game objects and call them e.g. `InteractionHand Left` and `InteractionHand Right`
  - Add the `Interaction Hand` component to both
    - Drag the `InteractionManager` game object into the `Manager`
    - Set `Hand Data Mode` to `Player Left` or `Player Right` respectively
  - If they are not already there, drag these interaction hands into the `Interaction Controller Status` section of the `InteractionManager`
- Game objects can interact with the hands via the `Interaction Behaviour` component or its subclasses (e.g. `Interaction Button`), see the official docs for more.

### Using the Database

(instructions possibly subject to change) Currently, the database models reside in `Assets\Util\DB.cs` (be sure to carefully read the comment there if you plan to make any changes!). The `GameRecorder` class takes care of recording any and all gaze data and writing it to the databse. To use it, simply add
```csharp
recorder = new GameRecorder(new Model.Util.Game{
    Name = "My Very Cool Game";
    Version = 1;  // increment this when making significant changes to the game that impact analysis of previously recorded data
})
```
somewhere in the `Start()` of your game, and
```csharp
private void OnDestroy()
{
    recorder.Commit();
}
```
to commit the data when done. This by itself does not store any data - you must call `recorder.Update()` in your `Update()` whenever you wish to add new data (this is so you can define yourself when the recording starts and ends). This will then record all (if any) gaze data *since the last frame*.
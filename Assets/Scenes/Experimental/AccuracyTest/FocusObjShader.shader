// This shader implements Billboarding with perspective correction; this is
// equivalent to overlaying a 2D image after rendering the scene, but since
// Unity with stereo rendering makes that rather hard, it's implemented as a
// shader for 3D objects instead.
// 
// The effect is that the visual centre of the rendered object is always
// *exactly* (!) at the centre of the game object, which is important when
// trying to do accurate measurements or calibration.

Shader "Custom/FocusObjShader"
{
	Properties
	{
		_Color1("Color 1", Color) = (1,0,0,1)
		_Color2("Color 2", Color) = (0,1,0,1)
	}
    SubShader
    {
		Lighting Off
		Blend Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            // vertex shader
			struct appdata {
				float4 vertex : POSITION;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			struct v2f {
				fixed2 pos : TEXCOORD0;
				float4 clip_pos : SV_POSITION;

				UNITY_VERTEX_OUTPUT_STEREO
			};
			float _Scale;
			v2f vert(appdata i)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.clip_pos = UnityObjectToClipPos(i.vertex); 
				// Project ball to screen with perspective, subtract projection of cente
				// of ball. This identifies the quadrants of the ball such that the
				// colours can be applied accordingly in the fragment shader.
				// Since the fragment shader receives the linearly interpolated values
				// from its surrounding vertices, we cannot simply compute the colour
				// here since this would blur out the transition.
				float4 centre = UnityObjectToClipPos(float4(0, 0, 0, 1));
				float4 projected = (o.clip_pos / o.clip_pos.w) - (centre / centre.w);
				o.pos = projected.xy;
				return o;
            }

			fixed4 _Color1;
			fixed4 _Color2;

			// fragment shader
			fixed4 frag(v2f i) : SV_Target
			{
				// branch-free way of writing if (sign(x) == sign(y)) then color1 else color2
				fixed l = 0.5 * abs(sign(i.pos.y) - sign(i.pos.x));
				return lerp(_Color1, _Color2, l);
			}
			ENDCG
		}
    }
	Fallback Off
}

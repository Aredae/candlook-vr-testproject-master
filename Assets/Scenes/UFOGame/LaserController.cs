using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Statistics;

using Util;

// General gist of how this works:
//
// The UFO itself mostly follows basic Newtonian physics for its movement (see
// UFOController.cs). Its steering is controlled by a PID controller (see
// UFONavigator.cs), which attempts to move to and hold a fixed position. These
// positions are defined by the UFOWaypointController.cs. All of this can in
// turn be tuned through the variables exposed to the Unity Editor.
//
// This class controls the laser beams that target the UFO. Naively, as soon as
// the lasers hit the UFO, we could consider it as "hit" and move to the next
// waypoint. However, this is highly sensitive to eye jitter etc - we want the
// player to hold their gaze on the UFO for some small duration until we
// register a hit. Unfortunately, this is challenging with the inaccuracy of
// the eye tracker depth data, and also runs counter to the natrual human
// behaviour of looking around a bit. To remedy this, we maintain a "proximity
// factor" variable that is scaled up when the gazepoint is within the treshold
// distance of the UFO, and scaled down when it is outside. This means that a
// steady gaze leads to an exponential increase in the proximity factor, which
// in turn leads to quickly registering a "hit". On the other hand, if the gaze
// is lost momentarily, we do not completely reset all the hard work performed
// by the player, but instead allow them to recover gently. Finally, to better
// compensate for sensor inaccuracy, we also provide an "aim assist" in form of
// a positive feedback lop: The larger the proximity factor, the more we
// artificially shift the gazepoint towards the UFO.
//
// All of this is a balancing act between making it work at all, and getting
// useful measurements. This balancing act can be tuned by tuning the variables
// here; I am confident that there exists some set of parameters that provides
// a reasonable tradeoff.

public class LaserController : MonoBehaviour
{

    public VolumetricLines.VolumetricLineBehavior laserLeft;
    public VolumetricLines.VolumetricLineBehavior laserRight;
    public GameObject targetUFO;

    [Tooltip("Distance at which gaze is considered to be on target")]
    public float threshold = 0.15f;
    [Tooltip("Automatically focus laser towards target once close enough")]
    public bool aimAssist = true;
    [Tooltip("Intensity of the vibration animation when the lasers hit the UFO")]
    public float forceIntensity = 0.1f;

    private EyeTracker et;

    private float proximityFactor = 0.0f; // the proximity factor described above
    private float minProxFactor = 0.04f;  // clamp it to a min. value so that multiplicative scaling can grow beyond zero
    private float lineWidthScale;         // current width of the laser, this is just cosmetics to animate a "hit"

    void Start()
    {
        et = new VarjoET(Camera.main);
        lineWidthScale = laserLeft.LineWidth;
        Debug.Assert(laserRight.LineWidth == lineWidthScale);
    }

    private float sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Varjo.XR.VarjoEventManager.Instance.GetButtonDown(0))
            et.calibrate();
        if (Input.GetKeyDown(KeyCode.A)) // for debugging: artificially move the laser within reach
        {
            laserLeft.EndPos = targetUFO.transform.position + Stat.Gaussian(0.3f * threshold);
            laserRight.EndPos = targetUFO.transform.position + Stat.Gaussian(0.3f * threshold);
        }

        EyeData eyeData = et.getFreshestData();

        if (eyeData.valid)
        {
            laserLeft.gameObject.SetActive(true);
            laserRight.gameObject.SetActive(true);

            float dist = (eyeData.convergencePoint - targetUFO.transform.position).magnitude;
            float maxScale = 2f; // controls how fast proximityFactor is allowed to scale
            float lambda = sigmoid(1f - dist / threshold); // if dist < threshold, then 0 < lambda < e / (e + 1), else -1 < lambda < 0
            lambda = 1f + Time.deltaTime * maxScale * lambda;
            proximityFactor = Mathf.Clamp(lambda * proximityFactor, minProxFactor, 1f);
            Vector3 convPtLeft = eyeData.left.position + eyeData.approxFocusDist * eyeData.left.gazeDirection;
            Vector3 convPtRight = eyeData.right.position + eyeData.approxFocusDist * eyeData.right.gazeDirection;
            if (aimAssist)
            {
                laserLeft.EndPos = Vector3.Lerp(convPtLeft, targetUFO.transform.position, 0.7f * (proximityFactor - minProxFactor));
                laserRight.EndPos = Vector3.Lerp(convPtRight, targetUFO.transform.position, 0.7f * (proximityFactor - minProxFactor));
            }
            else
            {

                laserLeft.EndPos = convPtLeft;
                laserRight.EndPos = convPtRight;
            }

            UFOWaypointController waypointController = targetUFO.GetComponent<UFOWaypointController>();
            if (proximityFactor > 0.9f && waypointController.hasArrived)
            {
                waypointController.Next();
                proximityFactor *= 0.1f;
            }
        }
        else
        {
            laserLeft.gameObject.SetActive(false);
            laserRight.gameObject.SetActive(false);
            proximityFactor *= 0.9f;
        }
        float rampScale = 5f;
        float lw = Mathf.Clamp(lineWidthScale * (Mathf.Exp(rampScale * (proximityFactor - minProxFactor)) - 1f) / (Mathf.Exp(rampScale) - 1f),
            0.01f * lineWidthScale, lineWidthScale);
        laserLeft.LineWidth = lw;
        laserRight.LineWidth = lw;
    }

    private void FixedUpdate()
    {
        if (proximityFactor > 2f * minProxFactor)
        {
            Vector3 f = Stat.Gaussian(forceIntensity * Time.fixedDeltaTime * laserLeft.LineWidth / lineWidthScale);
            targetUFO.transform.position += f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// General gist of how this works:
//
// The UFO itself mostly follows basic Newtonian physics for its movement (see
// UFOController.cs). Its steering is controlled by a PID controller (defined
// in this file), which attempts to move to and hold a fixed position. These
// positions are defined by the UFOWaypointController.cs. All of this can in
// turn be tuned through the variables exposed to the Unity Editor.

class PID
{
    public Vector3 sp { get; set; } // setpoint
    public float Kp { get; set; }   // proportional gain
    public float Kd { get; set; }   // derivate gain
    public float Ki { get; set; }   // integral gain

    private Vector3 integral = Vector3.zero;

    public void reset()
    {
        integral = Vector3.zero;
    }

    public Vector3 output(Vector3 pv, Vector3 deriv) // deriv is derivative of pv (assume somewhat incorrectly that sp is fixed)
    {
        Vector3 err = sp - pv;
        Vector3 u = Kp * err - Kd * deriv + Ki * integral;
        integral = 0.9f * integral + Time.fixedDeltaTime * err; // simple trapezoidal integrator with exp. decay of accumulated error
        return u;
    }
}

[RequireComponent(typeof(UFOController))]
public class UFONavigator : MonoBehaviour
{
    public Vector3 target;

    [Header("Position PID Gains")]
    [Tooltip("Proportional Gain")]
    public float positKp;
    [Tooltip("Integral Gain")]
    public float positKi;
    [Tooltip("Derivative Gain")]
    public float positKd;

    [Header("Speed PID Gains")]
    [Tooltip("Proportional Gain")]
    public float speedKp;
    [Tooltip("Integral Gain")]
    public float speedKi;
    [Tooltip("Derivative Gain")]
    public float speedKd;

    // takes position variables, outputs desired speed
    PID positPid;
    // takes desired speed, outputs desired acceleration
    PID speedPid;

    private Rigidbody body;
    private UFOController ufo;

    void Start()
    {
        Varjo.XR.VarjoRendering.SetFaceLocked(false);

        body = GetComponent<Rigidbody>();
        ufo = GetComponent<UFOController>();

    }

    // Called on start or when the values change in the editor
    void OnValidate()
    {
        
        positPid = new PID
        {
            Kp = positKp,
            Kd = positKd,
            Ki = positKi,
        };
        speedPid = new PID
        {
            Kp = speedKp,
            Kd = speedKd,
            Ki = speedKi,
        };
    }

    private void FixedUpdate()
    {
        positPid.sp = target;
        speedPid.sp = positPid.output(gameObject.transform.position, body.velocity);
        Vector3 accel = ufo.throttle / body.mass * (gameObject.transform.localRotation * Vector3.up) / body.mass + Physics.gravity;
        Vector3 desiredAccel = speedPid.output(body.velocity, accel);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + desiredAccel, Color.red, duration: 0, depthTest: false);
        ufo.throttle = 0.9f * ufo.throttle + 0.1f * body.mass * (desiredAccel - Physics.gravity).magnitude;
        Vector3 thrustDir = (desiredAccel - Physics.gravity).normalized;
        Quaternion rot = Quaternion.FromToRotation(gameObject.transform.up, thrustDir);
        // NOTE: currently thrust is calculated for the unsmoothed rotation. This is incorrect and causes some wobble, but looks cute :)
        body.transform.rotation *= Quaternion.Lerp(Quaternion.identity, rot, 0.1f); // TODO: limit torque properly
        // body.AddRelativeTorque(0.1f * rot.eulerAngles, ForceMode.Impulse);
        Debug.DrawLine(target, gameObject.transform.position, Color.cyan, duration: 0, depthTest: false);
    }

    // Attempts at a smarter navigation by doing more math, sadly it just fails :(
    // void Update()
    // {
    //     // first, compute if we can stop in time; rest is standard PID controller math
    //     Vector3 sp = target - gameObject.transform.position;
    //     Vector3 pv = body.velocity;
    //     float phi = 12f;
    //     float a = 1f;
    //     float b = 2 * Vector3.Dot(Physics.gravity, pv.normalized);
    //     float c = Physics.gravity.sqrMagnitude - (phi * phi) / (body.mass * body.mass);
    //     float k = Mathf.Sqrt(b * b - 4f * a * c);
    //     if (-b + k > 0)
    //         k = (-b + k) / (2f * a);
    //     else
    //         k = (-b - k) / (2f * a);
    //     Assert.IsTrue(k >= 0);
    //     Vector3 stopThrust = -(Physics.gravity + k * pv.normalized);
    //     float stopAccel = Vector3.Dot(stopThrust + Physics.gravity, sp.normalized);
    //     float v = Vector3.Dot(pv, sp.normalized);
    //     float t0 = -v / stopAccel;
    //     float stopDist = t0 * (v + 0.5f * t0 * stopAccel);
    //     Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + stopThrust, Color.blue, duration: 0, depthTest: false);
    //     Debug.DrawLine(target, target - sp, Color.cyan, duration: 0, depthTest: false);
    //     Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + stopDist * pv.normalized, Color.red, duration: 0, depthTest: false);
    //     if (!stopNow && stopDist >= 0.9f * sp.magnitude)
    //     {
    //         stopNow = true;
    //         lastDist = float.PositiveInfinity;
    //     }
    //     if (stopNow && lastDist >= sp.magnitude && pv.magnitude > 0.01f) {
    //         Debug.Log("Predicted stop now.");
    //         spscale *= 0.1f;
    //         sp = Vector3.zero;
    //         stopNow = true;
    //         lastDist = sp.magnitude;
    //         integral = Vector3.zero;
    //         gameObject.transform.localRotation = Quaternion.LookRotation(gameObject.transform.forward, stopThrust);
    //         ufo.throttle = phi;
    //         return;
    //     } else {
    //         spscale = Mathf.Clamp(0.2f + spscale / 0.5f, 0f, 1f);
    //         stopNow = false;
    //         lastDist = 0f;
    //     }

    //     sp -= 1f * pv;
    //     Vector3 e = sp - pv;
    //     integral += Time.deltaTime * e; // simple trapezoidal quadrature
    //     // integral = Vector3.ClampMagnitude(integral, 5f * maxAccel);
    //     Vector3 deriv = -ufo.throttle * (gameObject.transform.localRotation * Vector3.up) / body.mass - Physics.gravity;

    //     // the acceleration to be applied
    //     Vector3 u = Kp * e + Ki * integral + Kd * deriv;
    //     // Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + u, Color.green, duration: 0, depthTest: false);

    //     // now compute the corresponding change in rotation that realises it
    //     Vector3 t = body.mass * (u - Physics.gravity);
    //     Vector3 tn = t.normalized;
    //     // clamp tilt angle to reasonable bounds using poor man's Givens rotation
    //     float theta = Mathf.PI / 3f;
    //     float cosTheta = Mathf.Cos(theta);
    //     float dot = tn.y;
    //     // FIXME: handle downwards thrust
    //     if (dot < -0.9f)
    //     {
    //         // t = Vector3.up;
    //         // ufo.throttle = 0;
    //         ufo.throttle = t.magnitude;
    //     }
    //     else if (dot < cosTheta)
    //     {
    //         // float lambda = (cosTheta - dot) / (1f - dot);
    //         // tn += lambda * (Vector3.up - tn);
    //         // Assert.AreApproximatelyEqual(cosTheta, Vector3.Dot(Vector3.up, tn.normalized));
    //         // ufo.throttle = Mathf.Clamp(t.y / tn.normalized.y, 0, Mathf.Infinity);
    //         // t = tn;
    //         ufo.throttle = t.magnitude;
    //     }
    //     else
    //     {
    //         ufo.throttle = t.magnitude;
    //     }

    //     // Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + t, Color.red, duration: 0, depthTest: false);
    //     gameObject.transform.localRotation = Quaternion.LookRotation(gameObject.transform.forward, t);
    // }
}

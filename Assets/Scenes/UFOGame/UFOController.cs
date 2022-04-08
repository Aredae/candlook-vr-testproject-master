using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// General gist of how this works:
//
// The UFO itself mostly follows basic Newtonian physics for its movement. This
// file implements that part of the math. Its steering is controlled by a PID
// controller (see UFONavigator.cs), which attempts to move to and hold a fixed
// position. These positions are defined by the UFOWaypointController.cs. All
// of this can in turn be tuned through the variables exposed to the Unity
// Editor.

[RequireComponent(typeof(Rigidbody))]
public class UFOController : MonoBehaviour
{
    public GameObject exhaust;

    public float throttle = 1.0f;
    public float maxThrottle = 15f;

    private Rigidbody body;
    private float throttleScale; // we want the exhaust object to (visually) scale relative to the initial scale of the throttle

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = 6f * Vector3.down;

        body = GetComponent<Rigidbody>();
        throttleScale = exhaust.transform.localScale.y;
    }

    // LateUpdate() because we want it to run after any changes to the navigation
    void LateUpdate()
    {
        this.throttle = Mathf.Clamp(throttle, 0f, maxThrottle);
        this.exhaust.transform.localScale = new Vector3(1f, this.throttleScale * (1f + throttle - Physics.gravity.magnitude), 1f);
    }

    void FixedUpdate()
    {
        body.AddForce(throttle * (gameObject.transform.localRotation * Vector3.up), ForceMode.Force);
    }
}

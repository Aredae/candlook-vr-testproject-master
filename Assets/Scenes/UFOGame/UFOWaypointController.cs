using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// General gist of how this works:
//
// The UFO itself mostly follows basic Newtonian physics for its movement (see
// UFOController.cs). Its steering is controlled by a PID controller (see
// UFONavigator.cs), which attempts to move to and hold a fixed position. This
// class takes care of defining these positions. All of this can in turn be
// tuned through the variables exposed to the Unity Editor.

[RequireComponent(typeof(UFONavigator))]
public class UFOWaypointController : MonoBehaviour
{
    [Tooltip("Waypoints are relative to camera.")]
    public bool cameraRelative = false;
    public List<Vector3> waypoints;
    [Tooltip("Whether to switch waypoints based on hold-time automatically.")]
    public bool timeBased = true;
    [Tooltip("Time in seconds to wait at each waypoint.")]
    [Util.ConditionalField("timeBased")]
    public float holdDuration;

    private UFONavigator nav;
    private Rigidbody body;
    private int idx = 0;
    private float holdStartTime = 0;
    [HideInInspector]
    public bool hasArrived = false;


    void Start()
    {
        nav = this.GetComponent<UFONavigator>();
        body = this.GetComponent<Rigidbody>();
    }

    public void Next()
    {
        idx = (idx + 1) % waypoints.Count;
        hasArrived = false;
    }

    void Update()
    {
        if (!hasArrived && (gameObject.transform.position - nav.target).sqrMagnitude < 0.01f && body.velocity.sqrMagnitude < 0.01f)
        {
            hasArrived = true;
            holdStartTime = Time.time;
        }
        if (timeBased && hasArrived && Time.time - holdStartTime > holdDuration)
        {
            Next();
        }

        nav.target = cameraRelative ? Camera.main.transform.position + waypoints[idx] : waypoints[idx];
    }
}

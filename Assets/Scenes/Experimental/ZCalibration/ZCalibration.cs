using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Util;

// This file evolved out of numerous attempts at calibrating the ET data
// further to mitigate depth perception error. In its current form, it models a
// more-or-less constant, per-eye error in eye rotation and position, by
// computing an optimal (in the least-squares sense) rotational and positional
// offset from the on-screen calibration points and measured gaze data.

public class ZCalibration : MonoBehaviour
{
    private EyeTracker et;
    private GameRecorder recorder;
    // Start is called before the first frame update
    void Start()
    {
        et = new VarjoET(Camera.main);
        GazeVisualizer.spawn(et);

        recorder = new GameRecorder(new Util.Model.Game
        {
            Name = "Z-Calibration",
            Version = 1,
        }, et);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            et.calibrate();
        recorder.Update();
    }

    private void OnDestroy()
    {
        recorder.Commit();
    }
}

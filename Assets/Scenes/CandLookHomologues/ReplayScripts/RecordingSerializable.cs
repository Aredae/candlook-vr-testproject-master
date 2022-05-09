using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecordingSerializable
{
    public string RecordingId { get; set; }
    public GameSerializable Game { get; set; }
    public int Subject_id { get; set; }

    public string recordingtime { get; set; }
    // These lists will be translated by Npgsql to use PostgreSQL arrays, which can be efficiently
    // read by psycopg2 into numpy arrays for data analysis. This is considerably more efficient
    // than using a single row for every entry.

    // timestamp in nanoseconds
    public List<long> TimestampNS { get; set; } = new List<long>();
    // world-space
    public List<float> LeftEyePosX { get; set; } = new List<float>();
    public List<float> LeftEyePosY { get; set; } = new List<float>();
    public List<float> LeftEyePosZ { get; set; } = new List<float>();
    public List<float> RightEyePosX { get; set; } = new List<float>();
    public List<float> RightEyePosY { get; set; } = new List<float>();
    public List<float> RightEyePosZ { get; set; } = new List<float>();
    // world-space
    public List<float> LeftGazeDirX { get; set; } = new List<float>();
    public List<float> LeftGazeDirY { get; set; } = new List<float>();
    public List<float> LeftGazeDirZ { get; set; } = new List<float>();
    public List<float> RightGazeDirX { get; set; } = new List<float>();
    public List<float> RightGazeDirY { get; set; } = new List<float>();
    public List<float> RightGazeDirZ { get; set; } = new List<float>();
    // head-pose relative
    public List<float> LeftGazeDirRelX { get; set; } = new List<float>();
    public List<float> LeftGazeDirRelY { get; set; } = new List<float>();
    public List<float> LeftGazeDirRelZ { get; set; } = new List<float>();
    public List<float> RightGazeDirRelX { get; set; } = new List<float>();
    public List<float> RightGazeDirRelY { get; set; } = new List<float>();
    public List<float> RightGazeDirRelZ { get; set; } = new List<float>();
}

[Serializable]
public class GameSerializable
{
    public string Name { get; set; }
    public uint Version { get; set; }
}
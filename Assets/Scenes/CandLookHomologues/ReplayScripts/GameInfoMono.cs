using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoMono : MonoBehaviour
{
    public string GameName { get; set; }
    public int Version { get; set; }
    public DateTime timestamp { get; set; }

    public GameInfoMono(string name, int version, DateTime timestamp)
    {
        this.GameName = name;
        this.Version = version;
        this.timestamp = timestamp;
    }
}

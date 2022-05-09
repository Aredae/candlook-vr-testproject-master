using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoMono : MonoBehaviour
{
    public string GameName { get; set; }
    public string Version { get; set; }
    public string timestamp { get; set; }

    public GameInfoMono(string name, string version, string timestamp)
    {
        this.GameName = name;
        this.Version = version;
        this.timestamp = timestamp;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameInfo
{
    // Start is called before the first frame update
    public string GameName { get; set; }
    public int Version { get; set; }

    public DateTime timestamp { get; set; }
    
}

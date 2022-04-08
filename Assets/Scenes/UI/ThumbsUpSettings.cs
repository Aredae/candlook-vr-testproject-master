using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbsUpSettings : MonoBehaviour
{
    public Leap.Unity.HandModelBase leftHandModel;
    public Leap.Unity.HandModelBase rightHandModel;

    [Tooltip("Dispatched when thumbs-up given to start the game.")]
    public UnityEngine.Events.UnityEvent OnStart;
}

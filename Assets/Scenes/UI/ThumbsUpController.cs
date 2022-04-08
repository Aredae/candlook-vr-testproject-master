using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbsUpController : MonoBehaviour
{
    public ThumbsUpSettings settings;

    [Header("UI")]
    public Canvas canvas;
    public UnityEngine.UI.Text text;
    public UnityEngine.UI.Image icon;

    [Header("Detectors")]
    public Leap.Unity.Detector detector;
    public Leap.Unity.ExtendedFingerDetector leftExtDetector;
    public Leap.Unity.FingerDirectionDetector leftDirDetector;
    public Leap.Unity.ExtendedFingerDetector rightExtDetector;
    public Leap.Unity.FingerDirectionDetector rightDirDetector;

    // Start is called before the first frame update
    void Start()
    {
        leftExtDetector.HandModel = settings.leftHandModel;
        leftDirDetector.HandModel = settings.leftHandModel;
        rightExtDetector.HandModel = settings.rightHandModel;
        rightDirDetector.HandModel = settings.rightHandModel;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ThumbsUp()
    {
        text.text = "Okay!";
        text.transform.localPosition = Vector3.zero;
        icon.gameObject.SetActive(false);
    }

    void ThumbsNoLongerUp()
    {
        // wait for the thumbs-up to end before actually starting
        canvas.gameObject.SetActive(false);
        settings.OnStart.Invoke();

        // deactive ourselves
        settings.gameObject.SetActive(false);
    }
}

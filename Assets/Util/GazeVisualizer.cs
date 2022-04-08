using System.Collections;
using UnityEngine;

namespace Util
{
    public class GazeVisualizer : MonoBehaviour
    {
        private Object gazePointTemplate;

        private GameObject leftEye;
        private GameObject rightEye;
        private GameObject trackingLoss;
        private GameObject parallel;

        private EyeTracker et;

        public static GazeVisualizer spawn(EyeTracker et)
        {
            GameObject obj = new GameObject("GazeVisualizer");
            GazeVisualizer viz = obj.AddComponent<GazeVisualizer>();
            viz.et = et;
            return viz;
        }

        // Use this for initialization
        void Start()
        {
            trackingLoss = DisplayIcon.spawn("tracking_loss");
            trackingLoss.SetActive(false);
            parallel = DisplayIcon.spawn("parallel");
            parallel.SetActive(false);

            this.gazePointTemplate = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Util/Assets/GazePoint.prefab", typeof(Object));
            leftEye = Object.Instantiate(gazePointTemplate, Vector3.zero, Quaternion.identity) as GameObject;
            Material leftEyeMat = leftEye.GetComponent<MeshRenderer>().material;
            leftEyeMat.SetColor("_Color", new Color(1f, 0f, 0f, 0.2f));
            rightEye = Object.Instantiate(gazePointTemplate, Vector3.zero, Quaternion.identity) as GameObject;
            Material rightEyeMat = rightEye.GetComponent<MeshRenderer>().material;
            rightEyeMat.SetColor("_Color", new Color(0f, 1f, 0f, 0.2f));
        }

        // Update is called once per frame
        void Update()
        {
            EyeData eyeData = et.getFreshestData();
            if (!eyeData.valid)
            {
                trackingLoss.SetActive(true);
                parallel.SetActive(false);
                leftEye.SetActive(false);
                rightEye.SetActive(false);
            } else
            {
                leftEye.transform.localPosition = eyeData.left.position + eyeData.approxFocusDist * eyeData.left.gazeDirection;
                rightEye.transform.localPosition = eyeData.right.position + eyeData.approxFocusDist * eyeData.right.gazeDirection;
                trackingLoss.SetActive(false);
                leftEye.SetActive(true);
                rightEye.SetActive(true);
                if (eyeData.approxFocusDist >= EyeData.MAX_FOCUS_DIST)
                    parallel.SetActive(true);
                else
                    parallel.SetActive(false);
            }
        }
    }
}
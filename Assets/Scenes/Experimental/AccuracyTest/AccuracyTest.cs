using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Util;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

// This class implements a custom variation of GazeMetrics that allows us to
// quickly test sensor features such as accuracy, precision, field of view,
// etc.


public class AccuracyTest : MonoBehaviour
{
    private EyeTracker et;
    private GameRecorder recorder;

    public GameObject calibrationMarker;

    public List<GameObject> calibrationPoints;
    [Tooltip("Duration in seconds for which each calibration point is visible.")]
    public float visibleDuration = 1.5f;
    [Tooltip("Time in seconds for which data is ignored after initial appearance of a calibration point.")]
    public float startCutoff = 0.5f;
    [Tooltip("Time in seconds for which data is ignored before end of display time of a calibration point.")]
    public float endCutoff = 0.25f;

    private GazeVisualizer gazeViz;

    private enum State
    {
        Inactive,
        Preview,
        Calibrating,
        Waiting,
        Measuring,
        Done
    };
    State state = State.Inactive;
    float startTime = 0;

    private List<Tuple<Vector<double>, Vector<double>>> calibDataLeft;
    private List<Tuple<Vector<double>, Vector<double>>> calibDataRight;

    bool isRotCalibrated = false;
    private Matrix4x4? rotLeft = null;
    private Matrix4x4? rotRight = null;

    private GameObject[] texts;

    private class Measurement
    {
        List<Vector<double>> leftGaze = new List<Vector<double>>();
        List<Vector<double>> rightGaze = new List<Vector<double>>();
        List<Vector<double>> leftShould = new List<Vector<double>>();
        List<Vector<double>> rightShould = new List<Vector<double>>();

        public void Add(Vector<double> lg, Vector<double> ls, Vector<double> rg, Vector<double> rs)
        {
            this.leftGaze.Add(lg);
            this.leftShould.Add(ls);
            this.rightGaze.Add(rg);
            this.rightShould.Add(rs);
        }

        public Tuple<float, float, float, float> summarise()
        {
            Vector<double> leftGazeMean = this.leftGaze.Mean();
            float precLeft = (float)Math.Sqrt(leftGaze.Select(x => (x - leftGazeMean).SquaredL2Norm()).Mean());
            Vector<double> rightGazeMean = this.rightGaze.Mean();
            float precRight = (float)Math.Sqrt(rightGaze.Select(x => (x - rightGazeMean).SquaredL2Norm()).Mean());

            float accLeft = (float)Math.Sqrt(this.leftGaze.Zip(this.leftShould, (x1, x2) =>
           {
               double diff_x = Math.Atan2(x1[2], x1[0]) - Math.Atan2(x2[2], x2[0]);
               diff_x = (diff_x / Math.PI) * 180;
               double diff_y = Math.Atan2(x1[2], x1[1]) - Math.Atan2(x2[2], x2[1]);
               diff_y = (diff_y / Math.PI) * 180;
               return diff_x * diff_x + diff_y * diff_y;
           }).Mean());
            float accRight = (float)Math.Sqrt(this.rightGaze.Zip(this.rightShould, (x1, x2) =>
           {
               double diff_x = Math.Atan2(x1[2], x1[0]) - Math.Atan2(x2[2], x2[0]);
               double diff_y = Math.Atan2(x1[2], x1[1]) - Math.Atan2(x2[2], x2[1]);
               return diff_x * diff_x + diff_y * diff_y;
           }).Mean());

            return new Tuple<float, float, float, float>(accLeft, precLeft, accRight, precRight);
        }
    };

    Measurement[] measurements;
    Tuple<float, float, float, float>[] summaries;

    // Start is called before the first frame update
    void Start()
    {
        et = new VarjoET(Camera.main);
        et.calibrate();

        Varjo.XR.VarjoRendering.SetFaceLocked(true);

        gazeViz = GazeVisualizer.spawn(et);
        gazeViz.gameObject.SetActive(false);

        recorder = new GameRecorder(new Util.Model.Game
        {
            Name = "accuracy_test",
            Version = 1,
        }, et);

        foreach (GameObject obj in calibrationPoints)
            obj.SetActive(false);
        calibrationMarker.SetActive(false);

        this.texts = calibrationPoints.Select(p =>
        {
            GameObject tobj = new GameObject("Metrics");
            tobj.SetActive(false);
            tobj.transform.parent = p.transform;
            tobj.transform.localPosition = new Vector3(0f, 0.7f, 0f);
            tobj.transform.localScale = Vector3.one;
            TextMesh tmesh = tobj.AddComponent<TextMesh>();

            tmesh.characterSize = 0.2f;
            tmesh.anchor = TextAnchor.LowerCenter;
            tmesh.color = new Color(1f, 216f / 255f, 0f);

            return tobj;
        }).ToArray();
    }

    void transition(State into)
    {
        State before = this.state;
        State after = into;
        this.state = into;
        startTime = Time.time;


        if (before == State.Preview)
        {
            foreach (GameObject obj in this.calibrationPoints)
                obj.SetActive(false);
        }
        if (before == State.Done)
        {
            foreach (GameObject obj in texts)
                obj.SetActive(false);
            foreach (GameObject obj in calibrationPoints)
                obj.SetActive(false);
        }

        if (after == State.Preview)
        {
            foreach (GameObject obj in this.calibrationPoints)
                obj.SetActive(true);
        }
        if (after == State.Calibrating)
        {
            foreach (GameObject obj in calibrationPoints)
                obj.SetActive(false);
            this.calibDataLeft = new List<Tuple<Vector<double>, Vector<double>>>();
            this.calibDataRight = new List<Tuple<Vector<double>, Vector<double>>>();
        }
        if (after == State.Waiting)
        {
            foreach (GameObject obj in this.calibrationPoints)
                obj.SetActive(false);
        }
        if (after == State.Measuring)
        {
            this.measurements = new Measurement[this.calibrationPoints.Count];
            for (int i = 0; i < this.calibrationPoints.Count; ++i)
            {
                this.measurements[i] = new Measurement();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        recorder.Update();

        if (Input.GetKeyDown(KeyCode.V))
        {
            this.gazeViz.gameObject.SetActive(!this.gazeViz.gameObject.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.et.calibrate();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (this.isRotCalibrated)
                this.et.applyRotationalCalibration(null, null);
            else
                this.et.applyRotationalCalibration(rotLeft, rotRight);
            this.isRotCalibrated = !this.isRotCalibrated;
            calibrationMarker.SetActive(this.isRotCalibrated);
        }
        if (Input.GetKeyDown(KeyCode.P) && (state == State.Inactive || state == State.Preview))
        {
            transition(state == State.Preview ? State.Inactive : State.Preview);
        }
        if (Input.GetKeyDown(KeyCode.Space) && (state == State.Inactive || state == State.Preview))
        {
            transition(State.Calibrating);
        }
        if (Input.GetKeyDown(KeyCode.Space) && state == State.Done)
        {
            transition(State.Inactive);
        }
        if (Input.GetKeyDown(KeyCode.M) && (state == State.Inactive || state == State.Preview))
        {
            transition(State.Measuring);
        }

        if (state == State.Calibrating)
        {
            int idx = (int)((Time.time - startTime) / visibleDuration);
            if (idx >= calibrationPoints.Count)
            {
                finishCalibration();
                transition(State.Waiting);
            }
            else
            {
                GameObject obj = calibrationPoints[idx];
                obj.SetActive(true);
                if (idx > 0) calibrationPoints[idx - 1].SetActive(false);
                float time_in_obj = (Time.time - startTime) % visibleDuration;
                if (startCutoff < time_in_obj && time_in_obj < visibleDuration - endCutoff)
                {
                    // use data. Note that we use only a single ET point per frame since there is no camera information about older ET points
                    EyeData eyeData = et.getFreshestData();
                    Vector<double> p_left = eyeData.left.gazeDirection.normalized.ToMathNetDouble();
                    Vector<double> p_right = eyeData.right.gazeDirection.normalized.ToMathNetDouble();
                    Vector<double> q_left = (obj.transform.position - eyeData.left.position).normalized.ToMathNetDouble();
                    Vector<double> q_right = (obj.transform.position - eyeData.right.position).normalized.ToMathNetDouble();

                    this.calibDataLeft.Add(new Tuple<Vector<double>, Vector<double>>(p_left, q_left));
                    this.calibDataRight.Add(new Tuple<Vector<double>, Vector<double>>(p_right, q_right));
                }
            }
        }
        if (state == State.Waiting)
        {
            if (Time.time - startTime > 2 * visibleDuration)
                transition(State.Measuring);
        }
        if (state == State.Measuring)
        {
            int idx = (int)((Time.time - startTime) / visibleDuration);
            if (idx >= this.calibrationPoints.Count)
            {
                this.summaries = this.measurements.Select(m => m.summarise()).ToArray();
                for (int i = 0; i < this.calibrationPoints.Count; ++i)
                {
                    TextMesh tmesh = texts[i].GetComponent<TextMesh>();
                    tmesh.text = String.Format("A{0:F3}  A{2:F3}\nP{1:F3}  P{3:F3}",
                        summaries[i].Item1,
                        summaries[i].Item2,
                        summaries[i].Item3,
                        summaries[i].Item4);
                    texts[i].SetActive(true);
                    calibrationPoints[i].SetActive(true);
                }
                transition(State.Done);
            }
            else
            {
                GameObject obj = this.calibrationPoints[idx];
                obj.SetActive(true);
                if (idx > 0) this.calibrationPoints[idx - 1].SetActive(false);
                float time_in_obj = (Time.time - startTime) % visibleDuration;
                if (startCutoff < time_in_obj && time_in_obj < visibleDuration - endCutoff)
                {
                    EyeData eyeData = et.getFreshestData();
                    Vector<double> p_left = eyeData.left.gazeDirection.normalized.ToMathNetDouble();
                    Vector<double> p_right = eyeData.right.gazeDirection.normalized.ToMathNetDouble();
                    Vector<double> q_left = (obj.transform.position - eyeData.left.position).normalized.ToMathNetDouble();
                    Vector<double> q_right = (obj.transform.position - eyeData.right.position).normalized.ToMathNetDouble();

                    this.measurements[idx].Add(p_left, q_left, p_right, q_right);
                }
            }
        }
    }

    private void finishCalibration()
    {
        // this is so painfully verbose :(
        var means_left = calibDataLeft.Aggregate(new Tuple<Vector<double>, Vector<double>>(Vector.Build.Dense(3), Vector.Build.Dense(3)), (acc, d) =>
        {
            Vector<double> p = acc.Item1 + d.Item1;
            Vector<double> q = acc.Item2 + d.Item2;
            return new Tuple<Vector<double>, Vector<double>>(p, q);
        });
        means_left = new Tuple<Vector<double>, Vector<double>>(
            means_left.Item1 / calibDataLeft.Count,
            means_left.Item2 / calibDataLeft.Count
            );
        var means_right = calibDataRight.Aggregate(new Tuple<Vector<double>, Vector<double>>(Vector.Build.Dense(3), Vector.Build.Dense(3)), (acc, d) =>
        {
            Vector<double> p = acc.Item1 + d.Item1;
            Vector<double> q = acc.Item2 + d.Item2;
            return new Tuple<Vector<double>, Vector<double>>(p, q);
        });
        means_right = new Tuple<Vector<double>, Vector<double>>(
            means_right.Item1 / calibDataRight.Count,
            means_right.Item2 / calibDataRight.Count
            );

        Matrix<double> P_left = DenseMatrix.OfRowArrays(calibDataLeft.Select(x => (x.Item1 - means_left.Item1).AsArray()));
        Matrix<double> Q_left = DenseMatrix.OfRowArrays(calibDataLeft.Select(x => (x.Item2 - means_left.Item2).AsArray()));
        Matrix<double> P_right = DenseMatrix.OfRowArrays(calibDataLeft.Select(x => (x.Item1 - means_right.Item1).AsArray()));
        Matrix<double> Q_right = DenseMatrix.OfRowArrays(calibDataLeft.Select(x => (x.Item2 - means_right.Item2).AsArray()));

        var svd_left = (P_left.Transpose() * Q_left).Svd();
        var svd_right = (P_right.Transpose() * Q_right).Svd();

        bool orient_left = Math.Sign(svd_left.U.Determinant()) != Math.Sign(svd_right.VT.Determinant());
        bool orient_right = Math.Sign(svd_right.U.Determinant()) != Math.Sign(svd_right.VT.Determinant());

        Matrix<double> R_left = svd_left.VT;
        if (orient_left) R_left.SetRow(2, -1 * R_left.Row(2));
        R_left = svd_left.U * R_left;
        Vector<double> t_left = means_left.Item2 - R_left * means_left.Item1;
        this.rotLeft = R_left.ToUnity(t_left);

        Matrix<double> R_right = svd_right.VT;
        if (orient_right) R_right.SetRow(2, -1 * R_right.Row(2));
        R_right = svd_right.U * R_right;
        Vector<double> t_right = means_right.Item2 - R_right * means_right.Item1;
        this.rotRight = R_right.ToUnity(t_right);

        this.et.applyRotationalCalibration(rotLeft, rotRight);
        this.isRotCalibrated = true;
        calibrationMarker.SetActive(true);
    }

    private void OnDestroy()
    {
        recorder.Commit();
    }
}

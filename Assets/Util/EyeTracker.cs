using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Varjo.XR;
using UnityEngine;
using UnityEngine.Assertions;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Util
{
    public struct Eye
    {
        /// <summary>
        /// World position of the eye
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Normalized gaze direction vector in world coordinate system
        /// </summary>
        public Vector3 gazeDirection;
        /// <summary>
        /// Normalized gaze direction vector in head-pose relative coordinate system
        /// </summary>
        public Vector3 gazeDirectionRel;

        public float angleHorizWorld
        {
            get
            {
                return Mathf.Atan2(gazeDirection.x, gazeDirection.z);
            }
        }
        public float angleVertWorld
        {
            get
            {
                return Mathf.Atan2(gazeDirection.y, gazeDirection.z);
            }
        }

        public float angleHorizRel
        {
            get
            {
                return Mathf.Atan2(gazeDirectionRel.x, gazeDirectionRel.z);
            }
        }
        public float angleVertRel
        {
            get
            {
                return Mathf.Atan2(gazeDirectionRel.y, gazeDirectionRel.z);
            }
        }
    }

    public struct EyeData
    {
        /// <summary>
        /// This data is valid.
        /// </summary>
        /// <remarks>
        /// Data may be invalid for a number of reasons: Eye tracker cannot locate eyes, not 
        /// calibrated yet, currently calibrating, etc
        /// </remarks>
        public bool valid;
        /// <summary>
        /// Time in nanoseconds when the sample was taken
        /// </summary>
        public long timestamp_ns;
        public Eye left;
        public Eye right;
        public Eye average;
        /// <summary>
        /// Approximate distance to the focus point
        /// </summary>
        /// <remarks>
        /// Guesstimated based on the closest point to the affine lines induced by the gaze of each
        /// eye. For near-parallel or divergent gazes, the distance is always set to 
        /// <c>MAX_FOCUS_DIST</c>.
        /// </remarks>
        public float approxFocusDist;

        /// <summary>
        /// The point of convergence, in world coordinates.
        /// </summary>
        public Vector3 convergencePoint { get
            {
                return average.position + approxFocusDist * 0.5f * (left.gazeDirection + right.gazeDirection);
            } }

        public static float MAX_FOCUS_DIST = 50;
        public static float MIN_FOCUS_DIST = 0.05f;
    }

    public abstract class EyeTracker
    {
        /// <summary>
        /// Returns the most recent data.
        /// </summary>
        /// <returns></returns>
        public abstract EyeData getFreshestData();
        /// <summary>
        /// Returns all eye tracking data since the last frame, may be empty.
        /// </summary>
        /// <returns></returns>
        public abstract List<EyeData> getFreshData();
        public abstract void calibrate();
        public abstract void calibrateDepthScale();

        public abstract void applyRotationalCalibration(Matrix4x4? left, Matrix4x4? right);
    }

    // FIXME: ensure there is only ever one instance
    public class VarjoET : EyeTracker
    {
        private Camera camera;
        private long lastVarjoFrameNumber = 0;
        private long lastUnityFrameNumber = 0; // for knowing when to look for fresh data
        private List<EyeData> lastData;

        private Matrix4x4? rotLeft = null;
        private Matrix4x4? rotRight = null;

        public override void applyRotationalCalibration(Matrix4x4? left, Matrix4x4? right)
        {
            this.rotLeft = left;
            this.rotRight = right;
        }

        public VarjoET(Camera camera)
        {
            this.camera = camera;

            // Use some noise smoothing
            VarjoEyeTracking.SetGazeOutputFilterType(VarjoEyeTracking.GazeOutputFilterType.Standard);
        }

        public override void calibrate()
        {
            VarjoEyeTracking.RequestGazeCalibration(VarjoEyeTracking.GazeCalibrationMode.Legacy);
        }

        public override void calibrateDepthScale()
        {
            // should start the z calibration scene
            throw new System.NotImplementedException();
        }

        private Eye gazeRayToEye(VarjoEyeTracking.GazeData gazeData, VarjoEyeTracking.GazeRay gazeRay, Matrix4x4? rot)
        {
            Eye eye = new Eye();
            // FIXME: actually, might need to account for different units of length in the two coordinate systems?
            eye.position = camera.transform.TransformPoint(gazeRay.origin);
            eye.gazeDirectionRel = rot != null ? rot.Value.MultiplyPoint3x4(gazeRay.forward) : gazeRay.forward;
            eye.gazeDirection = camera.transform.TransformDirection(eye.gazeDirectionRel);
            return eye;
        }

        // KLUDGE: Modifies state (lastFrameNumber), which is counter-intuitive
        private EyeData toEyeData(VarjoEyeTracking.GazeData v_gazeData)
        {
            if (this.lastVarjoFrameNumber > 0 && v_gazeData.frameNumber > this.lastVarjoFrameNumber + 1)
            {
                // Not necessarily an issue if e.g. rendering is just slow
                // Debug.LogWarningFormat("Dropped {0} frames", v_gazeData.frameNumber - this.lastVarjoFrameNumber - 1);
            }
            this.lastVarjoFrameNumber = v_gazeData.frameNumber;

            EyeData eyeData = new EyeData();
            switch (v_gazeData.status)
            {
                case VarjoEyeTracking.GazeStatus.Invalid:
                    Debug.LogWarning("Cannot locate eyes - adjust the headset.");
                    eyeData.valid = false;
                    break;
                case VarjoEyeTracking.GazeStatus.Adjust:
                    Debug.LogWarning("Gaze data temporarily unavailable - currently calibrating.");
                    eyeData.valid = false;
                    break;
                case VarjoEyeTracking.GazeStatus.Valid:
                    eyeData.valid = true;
                    break;
                default:
                    Assert.IsFalse(false, "unreachable");
                    break;
            }
            eyeData.timestamp_ns = v_gazeData.captureTime;
            eyeData.left = gazeRayToEye(v_gazeData, v_gazeData.left, this.rotLeft);
            eyeData.right = gazeRayToEye(v_gazeData, v_gazeData.right, this.rotRight);
            eyeData.average = gazeRayToEye(v_gazeData, v_gazeData.gaze, null); // TODO: apply calibration here as well...

            // eyeData.approxFocusDistRel = v_gazeData.focusDistance; // clamped between 0.01 and 2.0, we want to try to do better

            // compute focus distance. Relies on v_gazeData.left.forward (and right.forward) being normalized
            if (Vector3.Dot(eyeData.left.gazeDirection, eyeData.right.gazeDirection) > 1f - 1e-6)
            {
                // essentially parallel gaze rays
                Debug.LogWarning("Parallel gaze");
                eyeData.approxFocusDist = EyeData.MAX_FOCUS_DIST;
            }
            else
            {
                Vector3 perp = Vector3.Cross(eyeData.left.gazeDirection, eyeData.right.gazeDirection);
                Matrix<float> A = DenseMatrix.OfColumnVectors(v_gazeData.left.forward.ToMathNetSingle(), -v_gazeData.right.forward.ToMathNetSingle(), perp.ToMathNetSingle());
                Vector<float> focusCoords = A.Solve(v_gazeData.right.origin.ToMathNetSingle() - v_gazeData.left.origin.ToMathNetSingle());
                eyeData.approxFocusDist = 0.5f * (focusCoords[0] + focusCoords[1]);
                if (float.IsNaN(eyeData.approxFocusDist) || eyeData.approxFocusDist < EyeData.MIN_FOCUS_DIST || eyeData.approxFocusDist > EyeData.MAX_FOCUS_DIST)
                    // rationale for MIN_FOCUS_DIST: convergence point appears to be behind the physical limits,
                    // hence it is more likely that the eyes are divering, so set the focus distance to "infinity" (max_focus_dist)
                    eyeData.approxFocusDist = EyeData.MAX_FOCUS_DIST;
            }

            return eyeData;
        }

        public override EyeData getFreshestData()
        {
            if (Time.frameCount > this.lastUnityFrameNumber)
            {
                this.getFreshData();
            }
            return this.lastData.DefaultIfEmpty(toEyeData(VarjoEyeTracking.GetGaze())).Last();
        }

        public override List<EyeData> getFreshData()
        {
            if (Time.frameCount == this.lastUnityFrameNumber)
            {
                return this.lastData;
            }
            else
            {
                List<VarjoEyeTracking.GazeData> l;
                VarjoEyeTracking.GetGazeList(out l);
                this.lastData = l.Select(toEyeData).ToList();
                this.lastUnityFrameNumber = Time.frameCount;
                return this.lastData;
            }
        }
    }
}

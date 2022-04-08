using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Stubs to get this project to build so that the migration tools can run
namespace Util
{
    public struct Vector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
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
    }

    public class EyeData
    {
        public bool valid;
        public long timestamp_ns;
        public Eye left;
        public Eye right;
        public Eye average;
        public float approxFocusDistRel;

        public static float MAX_FOCUS_DIST = 50;
    }
}

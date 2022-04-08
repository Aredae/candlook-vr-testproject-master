using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using S = MathNet.Numerics.LinearAlgebra.Single;
using D = MathNet.Numerics.LinearAlgebra.Double;

namespace Util
{
    public static class MathNETHelper
    {
        public static Vector3 ToUnity(this Vector<float> vec)
        {
            return new Vector3(vec[0], vec[1], vec[2]);
        }
        public static Vector3 ToUnity(this Vector<double> vec)
        {
            return new Vector3((float)vec[0], (float)vec[1], (float)vec[2]);
        }
        public static Matrix4x4 ToUnity(this Matrix<double> mathnet_mat)
        {
            Matrix4x4 mat = new Matrix4x4();
            if (mathnet_mat.RowCount == 3 && mathnet_mat.ColumnCount == 3)
            {
                mat[3, 3] = 1;
                for (int row = 0; row < 3; ++row)
                    for (int col = 0; col < 3; ++col)
                        mat[row, col] = (float)mathnet_mat[row, col];
            }
            else if (mathnet_mat.RowCount == 3 && mathnet_mat.ColumnCount == 3)
            {
                for (int row = 0; row < 4; ++row)
                    for (int col = 0; col < 4; ++col)
                        mat[row, col] = (float)mathnet_mat[row, col];
            }
            else
            {
                Debug.Assert(false, "Can only convert 3x3 or 4x4 matrices to unity Matrix4x4 class");
            }
            return mat;
        }
        public static Matrix4x4 ToUnity(this Matrix<double> mathnet_mat, Vector<double> t)
        {
            Debug.Assert(mathnet_mat.RowCount == 3 && mathnet_mat.ColumnCount == 3 && t.Count == 3);
            Matrix4x4 mat = new Matrix4x4();
            mat[3, 3] = 1;
            for (int row = 0; row < 3; ++row)
            {
                for (int col = 0; col < 3; ++col)
                    mat[row, col] = (float)mathnet_mat[row, col];
                mat[row, 3] = (float)t[row];
            }
            return mat;
        }

        public static Vector<double> ToMathNetDouble(this Vector3 vec)
        {
            return D.DenseVector.OfArray(new double[] { vec.x, vec.y, vec.z });
        }
        public static Vector<float> ToMathNetSingle(this Vector3 vec)
        {
            return S.DenseVector.OfArray(new float[] { vec.x, vec.y, vec.z });
        }

        public static double SquaredL2Norm(this Vector<double> vec)
        {
            return vec.DotProduct(vec);
        }
    }
}

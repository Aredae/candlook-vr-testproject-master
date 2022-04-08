using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using S = MathNet.Numerics.LinearAlgebra.Single;
using D = MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;

namespace Util
{
    public static class Stat
    {
        public static Vector<double> Mean(this IEnumerable<Vector<double>> iter)
        {

            return iter.Aggregate(D.Vector.Build.Dense(iter.First().Count), (acc, x) => acc + x) / iter.Count();
        }

        public static Vector3 Gaussian(float sigma)
        {
            return S.Vector.Build.Random(3, new Normal(0f, sigma)).ToUnity();
        }
        public static Vector3 Gaussian(Vector3 sigma)
        {
            return Gaussian(Vector3.zero, sigma);
        }
        public static Vector3 Gaussian(Vector3 mu, Vector3 sigma)
        {
            float x = (float) new Normal(mu.x, sigma.x).Sample();
            float y = (float) new Normal(mu.y, sigma.y).Sample();
            float z = (float) new Normal(mu.z, sigma.z).Sample();
            return new Vector3(x, y, z);
        }

        public static Vector3 Gaussian(Vector3 mu, Matrix<float> cov)
        {
            Matrix<double> M = mu.ToMathNetDouble().ToColumnMatrix();
            Matrix<double> U = cov.ToDouble();
            Matrix<double> V = D.DenseMatrix.CreateIdentity(1);
            Matrix<double> R = new MatrixNormal(M, U, V).Sample();
            return new Vector3((float) R[0, 0], (float) R[1, 0], (float) R[2, 0]);
        }
    }
}

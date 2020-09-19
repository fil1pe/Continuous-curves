using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curves
{
    public class BSpline : Curve
    {
        public int Degree { get; }
        public float[] Knots { get; }

        public BSpline(Point2D P0, Point2D P1, Point2D P2, Point2D P3, Point2D P4, Point2D P5, Color color)
            : base(color)
        {
            Degree = 3;

            ControlPoints.Add(P0);
            ControlPoints.Add(P1);
            ControlPoints.Add(P2);
            ControlPoints.Add(P3);
            ControlPoints.Add(P4);
            ControlPoints.Add(P5);

            Knots = new float[] { 0, 0, 0, 0, 1/3f, 2/6f, 1, 1, 1, 1 };
        }

        private float BasisFunction(int i, int j, float t)
        {
            if (j == 0)
                return Knots[i] <= t && t < Knots[i + 1] ? 1 : 0;

            float b0 = (t - Knots[i]) * BasisFunction(i, j - 1, t);
            if (b0 != 0) b0 /= Knots[i + j] - Knots[i];

            float b1 = (Knots[i + j + 1] - t) * BasisFunction(i + 1, j - 1, t);
            if (b1 != 0) b1 /= Knots[i + j + 1] - Knots[i + 1];

            return b0 + b1;
        }

        protected override float Evalf(List<float> x, float t)
        {
            return
                x[0] * BasisFunction(0, 3, t) +
                x[1] * BasisFunction(1, 3, t) +
                x[2] * BasisFunction(2, 3, t) +
                x[3] * BasisFunction(3, 3, t) +
                x[4] * BasisFunction(4, 3, t) +
                x[5] * BasisFunction(5, 3, t);
        }
    }
}

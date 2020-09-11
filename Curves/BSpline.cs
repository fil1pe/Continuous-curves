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
        public BSpline(Point2D P0, Point2D P1, Point2D P2, Point2D P3, Point2D P4, Color color)
            : base(P0, P1, P2, P3, P4, color) { }

        private float BasisFunction(int i, int j, float t)
        {
            if (j == 0)
                if (t >= i / 8f && t < (i + 1) / 8f)
                    return 1;
                else
                    return 0;
            return (
                (8 * t - i) * BasisFunction(i, j - 1, t) +
                (i + j + 1 - 8 * t) * BasisFunction(i + 1, j - 1, t)
            ) / j;
        }

        protected override float Evalf(float x0, float x1, float x2, float x3, float x4, float t)
        {
            return x0 * BasisFunction(0, 3, t) +
                x1 * BasisFunction(1, 3, t) +
                x2 * BasisFunction(2, 3, t) +
                x3 * BasisFunction(3, 3, t) +
                x4 * BasisFunction(4, 3, t);
        }
    }
}

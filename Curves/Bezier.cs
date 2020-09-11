using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Curves
{
    public class Bezier : Curve
    {
        public Bezier(Point2D P0, Point2D P1, Point2D P2, Point2D P3, Point2D P4, Color color)
            : base(P0, P1, P2, P3, P4, color) { }

        protected override float Evalf(float x0, float x1, float x2, float x3, float x4, float t)
        {
            return (float)(Pow(1 - t, 4) * x0 +
                4 * Pow(1 - t, 3) * t * x1 +
                6 * Pow(1 - t, 2) * t * t * x2 +
                4 * (1 - t) * t * t * t * x3 +
                t * t * t * t * x4);
        }
    }
}

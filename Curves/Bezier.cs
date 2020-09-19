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
        public int Degree { get; }

        public Bezier(Point2D P0, Point2D P1, Point2D P2, Point2D P3, Point2D P4, Color color)
            : base(color)
        {
            Degree = 4;

            ControlPoints.Add(P0);
            ControlPoints.Add(P1);
            ControlPoints.Add(P2);
            ControlPoints.Add(P3);
            ControlPoints.Add(P4);
        }

        protected override float Evalf(List<float> x, float t)
        {
            return (float)(Pow(1 - t, 4) * x[0] +
                4 * Pow(1 - t, 3) * t * x[1] +
                6 * Pow(1 - t, 2) * t * t * x[2] +
                4 * (1 - t) * t * t * t * x[3] +
                t * t * t * t * x[4]);
        }
    }
}

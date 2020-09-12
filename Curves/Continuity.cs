using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Math;

namespace Curves
{
    public partial class Window : Form
    {
        private void C0()
        {
            float deltaX = BSplineControlPoints.Last().Position.X - BezierControlPoints.First().Position.X;
            float deltaY = BSplineControlPoints.Last().Position.Y - BezierControlPoints.First().Position.Y;

            foreach (Point2D p in BezierControlPoints)
            {
                p.Position.X += deltaX;
                p.Position.Y += deltaY;
            }
        }

        private void G1()
        {
            C0();

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            float angle = 0;

            if (p1.Position.X - p0.Position.X <= 0 && p1.Position.Y - p0.Position.Y <= 0)
                angle = (float)PI;
            else if (p1.Position.X - p0.Position.X >= 0 && p1.Position.Y - p0.Position.Y <= 0)
                angle = (float)(PI / 2);
            else if (p1.Position.X - p0.Position.X <= 0 && p1.Position.Y - p0.Position.Y >= 0)
                angle = (float)(-PI / 2);

            BSplineCurve.Rotate(angle, p1.Position);

            if (p2.Position.X - p1.Position.X <= 0 && p2.Position.Y - p1.Position.Y <= 0)
                BezierCurve.Rotate((float)PI, p1.Position);
            else if (p2.Position.X - p1.Position.X >= 0 && p2.Position.Y - p1.Position.Y <= 0)
                BezierCurve.Rotate((float)(PI / 2), p1.Position);
            else if (p2.Position.X - p1.Position.X <= 0 && p2.Position.Y - p1.Position.Y >= 0)
                BezierCurve.Rotate((float)(-PI / 2), p1.Position);

            BezierCurve.Rotate(
                (float)(Atan(Abs(p2.Position.X - p1.Position.X) / Abs(p2.Position.Y - p1.Position.Y)) -
                Atan(Abs(p1.Position.X - p0.Position.X) / Abs(p1.Position.Y - p0.Position.Y))),
                p1.Position);

            BezierCurve.Rotate(-angle, p1.Position);
            BSplineCurve.Rotate(-angle, p1.Position);
        }

        private void C1()
        {
            G1();

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            float x1 = p1.Position.X,
                y1 = p1.Position.Y;

            float scaleX = (x1 - p0.Position.X) / (p2.Position.X - x1),
                scaleY = (y1 - p0.Position.Y) / (p2.Position.Y - y1);

            foreach (Point2D p in BezierControlPoints)
            {
                p.Position.X = scaleX * (p.Position.X - x1) + x1;
                p.Position.Y = scaleY * (p.Position.Y - y1) + y1;
            }
        }

        private void G2()
        {
            G1();
            // ...
        }

        private void C2()
        {
            C1();
            // ...
        }
    }
}

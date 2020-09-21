using System;
using System.Collections.Generic;
using System.Drawing;
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
            float deltaX = BSplineControlPoints.Last().Location.X - BezierControlPoints.First().Location.X;
            float deltaY = BSplineControlPoints.Last().Location.Y - BezierControlPoints.First().Location.Y;

            foreach (Point2D p in BezierControlPoints)
            {
                p.Location.X += deltaX;
                p.Location.Y += deltaY;
            }
        }

        private void G1()
        {
            C0();

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            float angle = 0;

            if (p1.Location.X - p0.Location.X <= 0 && p1.Location.Y - p0.Location.Y <= 0)
                angle = (float)PI;
            else if (p1.Location.X - p0.Location.X >= 0 && p1.Location.Y - p0.Location.Y <= 0)
                angle = (float)(PI / 2);
            else if (p1.Location.X - p0.Location.X <= 0 && p1.Location.Y - p0.Location.Y >= 0)
                angle = (float)(-PI / 2);

            BSplineCurve.Rotate(angle, p1.Location);

            if (p2.Location.X - p1.Location.X <= 0 && p2.Location.Y - p1.Location.Y <= 0)
                BezierCurve.Rotate((float)PI, p1.Location);
            else if (p2.Location.X - p1.Location.X >= 0 && p2.Location.Y - p1.Location.Y <= 0)
                BezierCurve.Rotate((float)(PI / 2), p1.Location);
            else if (p2.Location.X - p1.Location.X <= 0 && p2.Location.Y - p1.Location.Y >= 0)
                BezierCurve.Rotate((float)(-PI / 2), p1.Location);

            BezierCurve.Rotate(
                (float)(Atan(Abs(p2.Location.X - p1.Location.X) / Abs(p2.Location.Y - p1.Location.Y)) -
                Atan(Abs(p1.Location.X - p0.Location.X) / Abs(p1.Location.Y - p0.Location.Y))),
                p1.Location);

            BezierCurve.Rotate(-angle, p1.Location);
            BSplineCurve.Rotate(-angle, p1.Location);
        }

        private void C1()
        {
            G1();

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            BezierCurve.Scale(
                new PointF(
                    (p1.Location.X - p0.Location.X) / (p2.Location.X - p1.Location.X),
                    (p1.Location.Y - p0.Location.Y) / (p2.Location.Y - p1.Location.Y)
                ),
                p1.Location
            );

            float scale = BSplineCurve.Degree/(
                BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree] -
                BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1]
            );
            scale /= BezierCurve.Degree;

            BezierCurve.Scale(new PointF(scale, scale), p1.Location);
        }

        private void G2()
        {
            G1();

            float alpha = 1 / (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1]);
            float beta = 1 / (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree - 1] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 - 1]);

            PointF v0 = new PointF(
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Location.X,
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Location.Y
            );
            PointF v1 = new PointF(
               BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Location.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.X,
               BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Location.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.Y
            );
            PointF u0 = new PointF(
                BezierControlPoints[1].Location.X - BezierControlPoints[0].Location.X,
                BezierControlPoints[1].Location.Y - BezierControlPoints[0].Location.Y
            );
            PointF u1 = new PointF(
                BezierControlPoints[2].Location.X - BezierControlPoints[1].Location.X,
                BezierControlPoints[2].Location.Y - BezierControlPoints[1].Location.Y
            );

            if (Abs((alpha*v1.X - beta*v0.X)/(u1.X - u0.X) - (alpha*v1.Y - beta*v0.Y)/(u1.Y - u0.Y)) < 0.0001)
                return;

            BezierControlPoints[2].Location = new PointF(
                alpha * v1.X - beta * v0.X + u0.X + BezierControlPoints[1].Location.X,
                alpha * v1.Y - beta * v0.Y + u0.Y + BezierControlPoints[1].Location.Y
            );
        }
        private void C2()
        {
            C1();

            float alpha = (BSplineCurve.Degree - 1) * BSplineCurve.Degree;
            alpha /=
                    (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree - 2] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 - 1]) *
                    (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1]);
            float beta = (BSplineCurve.Degree - 1) * BSplineCurve.Degree;
            beta /=
                    (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree - 2] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 - 1]) *
                    (BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree - 1] -
                        BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 - 1]);
            float gamma = (BezierCurve.Degree - 1) * BezierCurve.Degree;

            PointF v0 = new PointF(
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Location.X,
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Location.Y
            );
            PointF v1 = new PointF(
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Location.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.X,
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Location.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Location.Y
            );
            PointF u0 = new PointF(
                BezierControlPoints[1].Location.X - BezierControlPoints[0].Location.X,
                BezierControlPoints[1].Location.Y - BezierControlPoints[0].Location.Y
            );

            BezierControlPoints[2].Location = new PointF(
                (alpha * v1.X - beta * v0.X + gamma * u0.X) / gamma + BezierControlPoints[1].Location.X,
                (alpha * v1.Y - beta * v0.Y + gamma * u0.Y) / gamma + BezierControlPoints[1].Location.Y
            );
        }
    }
}

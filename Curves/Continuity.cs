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

            BezierCurve.Scale(
                new PointF(
                    (p1.Position.X - p0.Position.X) / (p2.Position.X - p1.Position.X),
                    (p1.Position.Y - p0.Position.Y) / (p2.Position.Y - p1.Position.Y)
                ),
                p1.Position
            );

            float scale = BSplineCurve.Degree/(
                BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1 + BSplineCurve.Degree] -
                BSplineCurve.Knots[BSplineCurve.ControlPointsNum - 1]
            );
            scale /= BezierCurve.Degree;

            BezierCurve.Scale(new PointF(scale, scale), p1.Position);
        }

        private void G2()
        {
            G1();
            // ...
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
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Position.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Position.X,
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 1].Position.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Position.Y
            );
            PointF v1 = new PointF(
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Position.X - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Position.X,
                BSplineControlPoints[BSplineCurve.ControlPointsNum - 2].Position.Y - BSplineControlPoints[BSplineCurve.ControlPointsNum - 3].Position.Y
            );
            PointF u0 = new PointF(
                BezierControlPoints[0].Position.X - BezierControlPoints[1].Position.X,
                BezierControlPoints[0].Position.Y - BezierControlPoints[1].Position.Y
            );

            PointF p0 = BezierControlPoints[1].Position;
            Point2D p1 = BezierControlPoints[2];

            p1.Position = new PointF(
                (alpha*v0.X - beta*v1.X + gamma*u0.X) / gamma + p0.X,
                (alpha*v0.Y - beta*v1.Y + gamma*u0.Y) / gamma + p0.Y
            );
        }
    }
}

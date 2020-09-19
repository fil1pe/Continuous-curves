using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace Curves
{
    public abstract class Curve
    {
        protected List<Point2D> ControlPoints = new List<Point2D>();
        public int ControlPointsNum { get { return ControlPoints.Count; } }
        private Pen DrawPen;

        public Curve(Color color)
        {
            DrawPen = new Pen(color);
        }

        protected abstract float Evalf(List<float> x, float t);

        public PointF Eval(float t)
        {
            List<float> x = new List<float>(), y = new List<float>();
            foreach (Point2D p in ControlPoints)
            {
                x.Add(p.Location.X);
                y.Add(p.Location.Y);
            }

            return new PointF(Evalf(x, t), Evalf(y, t));
        }

        public void Draw(Graphics g)
        {
            const float precision = 1 / 100f;
            PointF p0 = Eval(0), p1;
            float t = precision;
            while (t <= 1)
            {
                p1 = Eval(t);
                g.DrawLine(DrawPen, p0, p1);
                p0 = p1;
                t += precision;
            }
        }

        public void Rotate(float angle, PointF point)
        {
            angle *= -1;
            foreach (Point2D p in ControlPoints)
            {
                float x = p.Location.X, y = p.Location.Y;
                x -= point.X;
                y -= point.Y;
                p.Location.X = (float)(Cos(angle) * x + Sin(angle) * y + point.X);
                p.Location.Y = (float)(-Sin(angle) * x + Cos(angle) * y + point.Y);
            }
        }

        public void Scale(PointF value, PointF point)
        {
            foreach (Point2D p in ControlPoints)
            {
                p.Location.X = value.X * (p.Location.X - point.X) + point.X;
                p.Location.Y = value.Y * (p.Location.Y - point.Y) + point.Y;
            }
        }
    }
}

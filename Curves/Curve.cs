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
        public List<Point2D> ControlPoints = new List<Point2D>();
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
                x.Add(p.Position.X);
                y.Add(p.Position.Y);
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
                float x = p.Position.X, y = p.Position.Y;
                x -= point.X;
                y -= point.Y;
                p.Position.X = (float)(Cos(angle) * x + Sin(angle) * y + point.X);
                p.Position.Y = (float)(-Sin(angle) * x + Cos(angle) * y + point.Y);
            }
        }

        public void Scale(PointF value, PointF point)
        {
            foreach (Point2D p in ControlPoints)
            {
                p.Position.X = value.X * (p.Position.X - point.X) + point.X;
                p.Position.Y = value.Y * (p.Position.Y - point.Y) + point.Y;
            }
        }
    }
}

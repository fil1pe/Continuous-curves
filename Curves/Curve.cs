using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curves
{
    public abstract class Curve
    {
        public Point2D P0, P1, P2, P3, P4;
        private Pen DrawPen;

        public Curve(Point2D P0, Point2D P1, Point2D P2, Point2D P3, Point2D P4, Color color)
        {
            this.P0 = P0;
            this.P1 = P1;
            this.P2 = P2;
            this.P3 = P3;
            this.P4 = P4;
            DrawPen = new Pen(color);
        }

        protected abstract float Evalf(float x0, float x1, float x2, float x3, float x4, float t);

        public PointF Eval(float t)
        {
            return new PointF(
                Evalf(P0.Position.X, P1.Position.X, P2.Position.X, P3.Position.X, P4.Position.X, t),
                Evalf(P0.Position.Y, P1.Position.Y, P2.Position.Y, P3.Position.Y, P4.Position.Y, t)
            );
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
    }
}

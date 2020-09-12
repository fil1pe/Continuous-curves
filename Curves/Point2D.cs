using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Curves
{
    public class Point2D
    {
        public PointF Position;
        private Brush DrawBrush;
        private bool Dragging = false;

        public Point2D(float x, float y, Color color, DrawPanel control)
        {
            Position = new PointF(x, y);
            DrawBrush = new SolidBrush(color);

            control.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (e.Location.X >= Position.X - 6 && e.Location.X <= Position.X + 6 &&
                    e.Location.Y >= Position.Y - 6 && e.Location.Y <= Position.Y + 6 &&
                    control.Dragging == false)
                {
                    Dragging = control.Dragging = true;
                }
            };

            control.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (Dragging)
                {
                    Position = e.Location;
                    control.Invalidate();
                }
            };

            control.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                Dragging = control.Dragging = false;
            };
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(DrawBrush, Position.X - 6, Position.Y - 6, 12, 12);
            g.DrawEllipse(Pens.Black, Position.X - 6, Position.Y - 6, 12, 12);
        }
    }
}

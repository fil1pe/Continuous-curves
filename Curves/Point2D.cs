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
        private PointF _position;
        public PointF Position { get { return _position; } }
        private Brush DrawBrush;
        private bool Dragging = false;

        public Point2D(float x, float y, Color color, Control control)
        {
            _position = new PointF(x, y);
            DrawBrush = new SolidBrush(color);

            control.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                if (e.Location.X >= Position.X - 6 && e.Location.X <= Position.X + 6 &&
                    e.Location.Y >= Position.Y - 6 && e.Location.Y <= Position.Y + 6)
                {
                    Dragging = true;
                }
            };

            control.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (Dragging)
                {
                    _position = e.Location;
                    control.Invalidate();
                }
            };

            control.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                Dragging = false;
            };
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(DrawBrush, Position.X - 6, Position.Y - 6, 12, 12);
            g.DrawEllipse(Pens.Black, Position.X - 6, Position.Y - 6, 12, 12);
        }
    }
}

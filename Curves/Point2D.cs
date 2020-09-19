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
        public PointF Location;
        public bool Active = true;
        private Brush DrawBrush;
        private bool Dragging = false;

        public Point2D(float x, float y, Color color, DrawPanel control)
        {
            Location = new PointF(x, y);
            DrawBrush = new SolidBrush(color);

            control.MouseDown += (sender, e) =>
            {
                if (e.Location.X >= Location.X - 6 && e.Location.X <= Location.X + 6 &&
                    e.Location.Y >= Location.Y - 6 && e.Location.Y <= Location.Y + 6 &&
                    !control.Dragging && Active)
                {
                    Dragging = control.Dragging = true;
                }
            };

            control.MouseMove += (sender, e) =>
            {
                if (Dragging)
                {
                    Location = e.Location;
                    control.Invalidate();
                }
            };

            control.MouseUp += (sender, e) =>
            {
                Dragging = control.Dragging = false;
            };
        }

        public void Draw(Graphics g)
        {
            g.FillEllipse(DrawBrush, Location.X - 6, Location.Y - 6, 12, 12);
            g.DrawEllipse(Pens.Black, Location.X - 6, Location.Y - 6, 12, 12);
        }
    }
}

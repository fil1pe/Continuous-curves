using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Math;

namespace Curves
{
    public class DrawPanel : Panel
    {
        public bool Dragging = false;

        public DrawPanel()
        {
            DoubleBuffered = true;
        }
    }

    public partial class Window : Form
    {
        private static float ParseFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }

        private List<Point2D> BSplineControlPoints = new List<Point2D>(),
            BezierControlPoints = new List<Point2D>();
        private BSpline BSplineCurve;
        private Bezier BezierCurve;

        public Window()
        {
            InitializeComponent();

            string[] fileLines = File.ReadAllLines(@"points");

            for (int i = 5; i < fileLines.Length; i++)
            {
                string[] coordinates = fileLines[i].Split(',');
                BSplineControlPoints.Add(
                    new Point2D(ParseFloat(coordinates[0]), ParseFloat(coordinates[1]), Color.Coral, panel1)
                );
            }
            BSplineCurve = new BSpline(BSplineControlPoints[0],
                BSplineControlPoints[1],
                BSplineControlPoints[2],
                BSplineControlPoints[3],
                BSplineControlPoints[4],
                BSplineControlPoints[5],
                Color.OrangeRed);

            for (int i = 0; i < 5; i++)
            {
                string[] coordinates = fileLines[i].Split(',');
                BezierControlPoints.Add(
                    new Point2D(ParseFloat(coordinates[0]), ParseFloat(coordinates[1]), Color.BlueViolet, panel1)
                );
            }
            BezierCurve = new Bezier(BezierControlPoints[0],
                BezierControlPoints[1],
                BezierControlPoints[2],
                BezierControlPoints[3],
                BezierControlPoints[4],
                Color.Blue);

            panel1.Paint += new PaintEventHandler(panel1_Paint);
            button1.Click += C0;
            button2.Click += G1;
            button3.Click += C1;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Curves:
            BezierCurve.Draw(e.Graphics);
            BSplineCurve.Draw(e.Graphics);

            // Polygons:
            Pen polygonPen = new Pen(Color.FromArgb(150, 150, 150, 150));
            polygonPen.DashStyle = DashStyle.Dash;

            for (int i = 0; i < BezierControlPoints.Count - 1; i++)
            {
                Point2D p1 = BezierControlPoints[i], p2 = BezierControlPoints[i + 1];
                e.Graphics.DrawLine(polygonPen, p1.Position, p2.Position);
            }
            for (int i = 0; i < BSplineControlPoints.Count - 1; i++)
            {
                Point2D p1 = BSplineControlPoints[i], p2 = BSplineControlPoints[i + 1];
                e.Graphics.DrawLine(polygonPen, p1.Position, p2.Position);
            }

            // Points:
            foreach (Point2D p in BezierControlPoints)
                p.Draw(e.Graphics);
            foreach (Point2D p in BSplineControlPoints)
                p.Draw(e.Graphics);
        }

        private void C0(object sender, EventArgs e)
        {
            float deltaX = BSplineControlPoints.Last().Position.X - BezierControlPoints.First().Position.X;
            float deltaY = BSplineControlPoints.Last().Position.Y - BezierControlPoints.First().Position.Y;

            foreach (Point2D p in BezierControlPoints)
            {
                p.Position.X += deltaX;
                p.Position.Y += deltaY;
            }

            panel1.Invalidate();
        }

        private void G1(object sender, EventArgs e)
        {
            C0(sender, e);

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            float angle = 0;

            if (p1.Position.X - p0.Position.X <= 0 && p1.Position.Y - p0.Position.Y <= 0)
                angle = (float)PI;
            else if (p1.Position.X - p0.Position.X >= 0 && p1.Position.Y - p0.Position.Y <= 0)
                angle = (float)(PI/2);
            else if (p1.Position.X - p0.Position.X <= 0 && p1.Position.Y - p0.Position.Y >= 0)
                angle = (float)(-PI/2);

            BSplineCurve.Rotate(angle, p1.Position);

            if (p2.Position.X - p1.Position.X <= 0 && p2.Position.Y - p1.Position.Y <= 0)
                BezierCurve.Rotate((float)PI, p1.Position);
            else if (p2.Position.X - p1.Position.X >= 0 && p2.Position.Y - p1.Position.Y <= 0)
                BezierCurve.Rotate((float)(PI/2), p1.Position);
            else if (p2.Position.X - p1.Position.X <= 0 && p2.Position.Y - p1.Position.Y >= 0)
                BezierCurve.Rotate((float)(-PI/2), p1.Position);

            BezierCurve.Rotate(
                (float)(Atan(Abs(p2.Position.X - p1.Position.X) / Abs(p2.Position.Y - p1.Position.Y)) -
                Atan(Abs(p1.Position.X - p0.Position.X) / Abs(p1.Position.Y - p0.Position.Y))),
                p1.Position);

            BezierCurve.Rotate(-angle, p1.Position);
            BSplineCurve.Rotate(-angle, p1.Position);

            panel1.Invalidate();
        }

        private void C1(object sender, EventArgs e)
        {
            G1(sender, e);

            Point2D p0 = BSplineControlPoints[BSplineControlPoints.Count - 2];
            Point2D p1 = BezierControlPoints.First();
            Point2D p2 = BezierControlPoints[1];

            float x1 = p1.Position.X,
                y1 = p1.Position.Y;

            float scaleX = (x1 - p0.Position.X) / (p2.Position.X - x1),
                scaleY = (y1 - p0.Position.Y) / (p2.Position.Y - y1);

            foreach (Point2D p in BezierControlPoints)
            {
                p.Position.X = scaleX*(p.Position.X - x1) + x1;
                p.Position.Y = scaleY*(p.Position.Y - y1) + y1;
            }

            panel1.Invalidate();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to save?", "Save before closing?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                List<string> points = new List<string>();
                foreach (Point2D p in BezierControlPoints)
                {
                    points.Add(p.Position.X + "," + p.Position.Y);
                }
                foreach (Point2D p in BSplineControlPoints)
                {
                    points.Add(p.Position.X + "," + p.Position.Y);
                }
                File.WriteAllLines(@"points", points);
            }
        }
    }
}

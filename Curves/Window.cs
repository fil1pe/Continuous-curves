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
            return float.Parse(str, CultureInfo.CurrentCulture);
        }

        private List<Point2D> BSplineControlPoints = new List<Point2D>(),
            BezierControlPoints = new List<Point2D>();
        private BSpline BSplineCurve;
        private Bezier BezierCurve;
        private bool DraggingPanel = false;
        private PointF MouseLocation;

        public Window()
        {
            InitializeComponent();

            string[] fileLines = File.ReadAllLines(@"points");

            for (int i = 5; i < fileLines.Length; i++)
            {
                string[] coordinates = fileLines[i].Split(';');
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
                string[] coordinates = fileLines[i].Split(';');
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
            button1.Click += (s, e) => { C0(); panel1.Invalidate(); };
            button2.Click += (s, e) => { G1(); panel1.Invalidate(); };
            button3.Click += (s, e) => { C1(); panel1.Invalidate(); };
            button4.Click += (s, e) => { G2(); panel1.Invalidate(); };
            button5.Click += (s, e) => { C2(); panel1.Invalidate(); };

            chBox.Click += (s, e) =>
            {
                ShowControlPoints = !ShowControlPoints;
                foreach (Point2D p in BSplineControlPoints)
                    p.Active = !p.Active;
                foreach (Point2D p in BezierControlPoints)
                    p.Active = !p.Active;
                panel1.Invalidate();
            };

            panel1.MouseWheel += MouseWheelEvent;
            panel1.MouseDown += (s, e) =>
            {
                if (panel1.Dragging) return;
                DraggingPanel = true;
                MouseLocation = e.Location;
            };
            panel1.MouseMove += (s, e) =>
            {
                if (!DraggingPanel) return;
                PointF newMousePos = e.Location;
                foreach (Point2D p in BSplineControlPoints)
                {
                    p.Location.X += newMousePos.X - MouseLocation.X;
                    p.Location.Y += newMousePos.Y - MouseLocation.Y;
                }
                foreach (Point2D p in BezierControlPoints)
                {
                    p.Location.X += newMousePos.X - MouseLocation.X;
                    p.Location.Y += newMousePos.Y - MouseLocation.Y;
                }
                MouseLocation = newMousePos;
                panel1.Invalidate();
            };
            panel1.MouseUp += (s, e) => { DraggingPanel = false; };
        }

        private bool ShowControlPoints = true;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Curves:
            BezierCurve.Draw(e.Graphics);
            BSplineCurve.Draw(e.Graphics);

            if (!ShowControlPoints) return;

            // Polygons:
            Pen polygonPen = new Pen(Color.FromArgb(150, 150, 150, 150));
            polygonPen.DashStyle = DashStyle.Dash;

            for (int i = 0; i < BezierControlPoints.Count - 1; i++)
            {
                Point2D p1 = BezierControlPoints[i], p2 = BezierControlPoints[i + 1];
                e.Graphics.DrawLine(polygonPen, p1.Location, p2.Location);
            }
            for (int i = 0; i < BSplineControlPoints.Count - 1; i++)
            {
                Point2D p1 = BSplineControlPoints[i], p2 = BSplineControlPoints[i + 1];
                e.Graphics.DrawLine(polygonPen, p1.Location, p2.Location);
            }

            // Points:
            foreach (Point2D p in BezierControlPoints)
                p.Draw(e.Graphics);
            foreach (Point2D p in BSplineControlPoints)
                p.Draw(e.Graphics);
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to save?", "Save before closing?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                List<string> points = new List<string>();
                foreach (Point2D p in BezierControlPoints)
                    points.Add(p.Location.X.ToString() + ";" + p.Location.Y.ToString());
                foreach (Point2D p in BSplineControlPoints)
                    points.Add(p.Location.X.ToString() + ";" + p.Location.Y.ToString());
                File.WriteAllLines(@"points", points);
            }
        }

        private void MouseWheelEvent(object sender, MouseEventArgs e)
        {
            float scale = e.Delta / 120 >= 1 ? 1.5f : 2/3f;

            BezierCurve.Scale(new PointF(scale, scale), e.Location);
            BSplineCurve.Scale(new PointF(scale, scale), e.Location);
            panel1.Invalidate();
        }
    }
}

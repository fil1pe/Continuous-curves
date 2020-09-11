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

        private List<Point2D> ControlPoints = new List<Point2D>();
        private Bezier BezierCurve;
        private BSpline BSplineCurve;

        public Window()
        {
            InitializeComponent();
            Color color = Color.Coral;
            List<string> pointsStr = File.ReadAllLines(@"points").ToList();
            for (int i=0; i<10; i++)
            {
                if (i == 5)
                    color = Color.BlueViolet;
                string[] coordinates = pointsStr[i].Split(',');
                ControlPoints.Add(
                    new Point2D(ParseFloat(coordinates[0]), ParseFloat(coordinates[1]), color, panel1)
                );
            }
            BSplineCurve = new BSpline(ControlPoints[0],
                ControlPoints[1],
                ControlPoints[2],
                ControlPoints[3],
                ControlPoints[4],
                Color.OrangeRed);
            BezierCurve = new Bezier(ControlPoints[5],
                ControlPoints[6],
                ControlPoints[7],
                ControlPoints[8],
                ControlPoints[9],
                Color.Blue);
            panel1.Paint += new PaintEventHandler(panel1_Paint);
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

            for (int i=0; i<4; i++)
            {
                Point2D p1 = ControlPoints[i], p2 = ControlPoints[i+1];
                e.Graphics.DrawLine(polygonPen, p1.Position, p2.Position);
            }
            for (int i=5; i<9; i++)
            {
                Point2D p1 = ControlPoints[i], p2 = ControlPoints[i + 1];
                e.Graphics.DrawLine(polygonPen, p1.Position, p2.Position);
            }

            // Points:
            for (int i=9; i>=0; i--)
                ControlPoints[i].Draw(e.Graphics);
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<string> points = new List<string>();
            foreach (Point2D p in ControlPoints)
            {
                points.Add(p.Position.X + "," + p.Position.Y);
            }
            File.WriteAllLines(@"points", points);
        }
    }
}

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
    public partial class Window : Form
    {
        class DrawPanel : Panel
        {
            public DrawPanel()
            {
                DoubleBuffered = true;
            }
        }

        private static float ParseFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }

        private List<Point2D> BezierControlPoints = new List<Point2D>();
        private Bezier BezierCurve;

        public Window()
        {
            InitializeComponent();
            Color color = Color.BlueViolet;
            List<string> pointsStr = File.ReadAllLines(@"points").ToList();
            for (int i=0; i<5; i++)
            {
                string[] coordinates = pointsStr[i].Split(',');
                BezierControlPoints.Add(
                    new Point2D(ParseFloat(coordinates[0]), ParseFloat(coordinates[1]), color, panel1)
                );
            }
            color = Color.Blue;
            BezierCurve = new Bezier(BezierControlPoints[0],
                BezierControlPoints[1],
                BezierControlPoints[2],
                BezierControlPoints[3],
                BezierControlPoints[4],
                color);
            panel1.Paint += new PaintEventHandler(panel1_Paint);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Pen polygonPen = new Pen(Color.FromArgb(150, 150, 150, 150));
            polygonPen.DashStyle = DashStyle.Dash;

            // Curves:
            BezierCurve.Draw(e.Graphics);

            // Points:
            for (int i=0; i<BezierControlPoints.Count-1; i++)
            {
                Point2D p1 = BezierControlPoints[i], p2 = BezierControlPoints[i+1];
                e.Graphics.DrawLine(polygonPen, p1.Position, p2.Position);
            }
            foreach (Point2D p in BezierControlPoints)
            {
                p.Draw(e.Graphics);
            }
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            List<string> points = new List<string>();
            foreach (Point2D p in BezierControlPoints)
            {
                points.Add(p.Position.X + "," + p.Position.Y);
            }
            File.WriteAllLines(@"points", points);
        }
    }
}

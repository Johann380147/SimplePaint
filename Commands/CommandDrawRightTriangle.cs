using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawRightTriangle : Command
    {
        Point startPoint = Point.Empty;
        Point endPoint = Point.Empty;
        PointF[] pointPolygon;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        public override void OnMouseUp(Graphics g, MouseEventArgs e)
        {
            g.DrawPolygon(new Pen(Brush), pointPolygon);
        }

        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            endPoint = e.Location;
        }

        public override void Paint(object sender, PaintEventArgs e)
        {
            PointF point1 = new PointF(startPoint.X, startPoint.Y);
            PointF point2 = new PointF(endPoint.X, endPoint.Y);
            PointF point3 = new PointF(startPoint.X, endPoint.Y);

            pointPolygon = new PointF[] { point1, point2, point3 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }
    }
}

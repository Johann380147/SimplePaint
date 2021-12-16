using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawArrowRight : Command
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
            PointF point3 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.6f), startPoint.Y);
            PointF point5 = new PointF(point3.X, endPoint.Y);
            PointF point4 = new PointF(endPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.5f));
            PointF point1 = new PointF(startPoint.X, point3.Y + ((point4.Y - point3.Y) * 0.5f));
            PointF point7 = new PointF(startPoint.X, point4.Y + ((point5.Y - point4.Y) * 0.5f));
            PointF point2 = new PointF(point3.X, point1.Y);
            PointF point6 = new PointF(point5.X, point7.Y);

            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }
    }
}

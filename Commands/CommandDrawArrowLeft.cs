using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawArrowLeft : Command
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
            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.4f), startPoint.Y);
            PointF point7 = new PointF(point2.X, endPoint.Y);
            PointF point1 = new PointF(startPoint.X, point2.Y + ((point7.Y - point2.Y) * 0.5f));
            PointF point4 = new PointF(endPoint.X, point2.Y + ((point1.Y - point2.Y) * 0.5f));
            PointF point5 = new PointF(endPoint.X, point1.Y + ((point7.Y - point1.Y) * 0.5f));
            PointF point3 = new PointF(point2.X, point4.Y);
            PointF point6 = new PointF(point3.X, point5.Y);

            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }
    }
}

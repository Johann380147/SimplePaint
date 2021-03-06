using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawArrowUp :Command
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
            PointF point1 = new PointF(startPoint.X, startPoint.Y + ((endPoint.Y - startPoint.Y) * 0.4f));
            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y);
            PointF point3 = new PointF(endPoint.X, point1.Y);
            PointF point4 = new PointF(point2.X + ((point3.X - point2.X)) * 0.5f, point3.Y);
            PointF point5 = new PointF(point4.X, endPoint.Y);
            PointF point7 = new PointF(point1.X + ((point2.X - point1.X) * 0.5f), point3.Y);
            PointF point6 = new PointF(point7.X, point5.Y);

            pointPolygon = new PointF[] { point1, point2, point3, point4, point5, point6, point7 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }
    }
}

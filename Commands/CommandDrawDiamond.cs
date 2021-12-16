using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawDiamond : Command
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
            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
            PointF point1 = new PointF(startPoint.X, point2.Y + ((endPoint.Y - point2.Y)) * 0.5f);
            PointF point3 = new PointF(endPoint.X, point1.Y);
            PointF point4 = new PointF(point2.X, endPoint.Y);

            pointPolygon = new PointF[] { point1, point2, point3, point4 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }
        
    }
}

using System;
using System.Drawing;


namespace Commands
{
    class CommandDrawTriangle : Command
    {
        Point startPoint = Point.Empty;
        Point endPoint = Point.Empty;
        PointF[] pointPolygon;

        public override void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            startPoint = e.Location;
            endPoint = e.Location;
        }

        public override void OnMouseUp(System.Drawing.Graphics g, System.Windows.Forms.MouseEventArgs e)
        {
            g.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }

        public override void OnMouseMove(System.Drawing.Graphics g, System.Windows.Forms.MouseEventArgs e, System.Drawing.Point startPoint)
        {
            endPoint = e.Location;
        }

        public override void Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            PointF point1 = new PointF(startPoint.X, endPoint.Y);
            PointF point2 = new PointF(startPoint.X + ((endPoint.X - startPoint.X) * 0.5f), startPoint.Y - 10);
            PointF point3 = new PointF(endPoint.X, endPoint.Y);

            pointPolygon = new PointF[] { point1, point2, point3 };
            e.Graphics.DrawPolygon(new Pen(Brush, Width), pointPolygon);
        }

    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    public class CommandDrawRectangle : Command
    {
        Point startPoint = Point.Empty;
        Point endPoint = Point.Empty;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        public override void OnMouseUp(Graphics g, MouseEventArgs e)
        {
            Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);
            g.DrawRectangle(GeneralPen, r);
        }

        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            endPoint = e.Location;
        }

        public override void Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(ImageHandler.RectAlgorithm(startPoint, endPoint).Item1,
                ImageHandler.RectAlgorithm(startPoint, endPoint).Item2);
            e.Graphics.DrawRectangle(GeneralPen, r);
        }
    }
}

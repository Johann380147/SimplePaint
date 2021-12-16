using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawLine : Command
    {
        Point startPoint = Point.Empty;
        Point endPoint = Point.Empty;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        public override void OnMouseUp(Graphics g, MouseEventArgs e)
        {
            g.DrawLine(GeneralPen, startPoint, endPoint);
        }

        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            endPoint = e.Location;
        }

        public override void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(GeneralPen, startPoint, endPoint);
        }
    }
}

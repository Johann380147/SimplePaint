using System;
using System.Drawing;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawSprayBrush : Command
    {
        Point startPoint = Point.Empty;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }


        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            g.DrawImage(SprayImage1, e.Location);
            startPoint = e.Location;
        }

    }
}

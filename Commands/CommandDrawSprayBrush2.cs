using System;
using System.Drawing;
using System.Windows.Forms;;

namespace Commands
{
    class CommandDrawSprayBrush2 : Command
    {
        Point startPoint = Point.Empty;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }


        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            g.DrawImage(SprayImage2, e.Location);
            startPoint = e.Location;
        }
    }
}

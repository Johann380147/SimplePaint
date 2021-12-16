using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Commands
{
    class CommandDrawTrans : Command
    {
        GraphicsPath graphicsPath;

        public override void OnMouseDown(object sender, MouseEventArgs e)
        {
            graphicsPath = new GraphicsPath();

        }

        public override void OnMouseUp(Graphics g, MouseEventArgs e)
        {
            g.DrawPath(TransPen, graphicsPath);
            graphicsPath.Dispose();

        }

        public override void OnMouseMove(Graphics g, MouseEventArgs e, Point startPoint)
        {
            graphicsPath.StartFigure();
            graphicsPath.AddLine(startPoint, e.Location);
            startPoint = e.Location;
        }

        public override void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawPath(TransPen, graphicsPath);
        }
    }
}

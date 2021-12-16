using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class cButton : Control
    {
        bool isMouseOver = false;
        bool isMousePressed = false;

        public cButton()
        {
            InitializeComponent();
            ClientSize = new Size(75, 30);
            Text = "Accept";
            Font = new Font(new FontFamily("Buxton Sketch"), 10f, FontStyle.Regular);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            GraphicsPath path = RoundedRectangle.Create(0, 0, ClientSize.Width, ClientSize.Height, 5);
            if (isMousePressed) e.Graphics.FillPath(new SolidBrush(Color.FromArgb(0, 0, 0)), path);
            else if (!isMouseOver) e.Graphics.FillPath(new SolidBrush(Color.FromArgb(44, 44, 44)), path);

            if (isMouseOver && !isMousePressed)
            {
                e.Graphics.FillPath(new SolidBrush(Color.FromArgb(22, 22, 22)), path);
                e.Graphics.DrawPath(new Pen(Color.RoyalBlue), path);
            }

            SizeF stringLength = e.Graphics.MeasureString(Text, Font);
            e.Graphics.DrawString(Text, Font, new SolidBrush(Color.WhiteSmoke),
                (Width - stringLength.Width) / 2, (Height - stringLength.Height) / 2);

            using (LinearGradientBrush brush = new LinearGradientBrush
                (new PointF(0, 0), new PointF((Width / 4) + 1, 0),
                Color.Transparent, Color.FromKnownColor(KnownColor.Control)))
            {
                e.Graphics.DrawLine(new Pen(brush), new PointF((Width * 0.25f) + 2, 1),
                    new PointF((Width / 2) + 2, 1));
                e.Graphics.DrawLine(new Pen(brush), new PointF((Width * 0.25f) + 2, Height - 1.8f),
                                new PointF((Width / 2) + 2, Height - 1.8f));
            }

            using (LinearGradientBrush brush = new LinearGradientBrush
                (new PointF(0, 0), new PointF((Width / 4), 0),
                Color.FromKnownColor(KnownColor.Control), Color.Transparent))
            {
                e.Graphics.DrawLine(new Pen(brush), new PointF((Width * 0.5f), 1),
                    new PointF(Width * 0.72f, 1));
                e.Graphics.DrawLine(new Pen(brush), new PointF((Width * 0.5f), Height - 1.8f),
                    new PointF(Width * 0.72f, Height - 1.8f));
            }
            ControlPaint.DrawBorder3D(this.CreateGraphics(), ClientRectangle, Border3DStyle.Adjust,
                Border3DSide.Right | Border3DSide.Bottom);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isMouseOver = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isMouseOver = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isMousePressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMousePressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        public void PerformClick()
        {
            this.InvokeOnClick(this, EventArgs.Empty);
        }
    }

    public abstract class RoundedRectangle
    {
        public enum RectangleCorners
        {
            None = 0, TopLeft = 1, TopRight = 2, BottomLeft = 4, BottomRight = 8,
            All = TopLeft | TopRight | BottomLeft | BottomRight
        }

        public static GraphicsPath Create(int x, int y, int width, int height,
                                          int radius, RectangleCorners corners)
        {
            int xw = x + width;
            int yh = y + height;
            int xwr = xw - radius;
            int yhr = yh - radius;
            int xr = x + radius;
            int yr = y + radius;
            int r2 = radius * 2;
            int xwr2 = xw - r2;
            int yhr2 = yh - r2;

            GraphicsPath p = new GraphicsPath();
            p.StartFigure();

            //Top Left Corner
            if ((RectangleCorners.TopLeft & corners) == RectangleCorners.TopLeft)
            {
                p.AddArc(x, y, r2, r2, 180, 90);
            }
            else
            {
                p.AddLine(x, yr, x, y);
                p.AddLine(x, y, xr, y);
            }

            //Top Edge
            p.AddLine(xr, y, xwr, y);

            //Top Right Corner
            if ((RectangleCorners.TopRight & corners) == RectangleCorners.TopRight)
            {
                p.AddArc(xwr2, y, r2, r2, 270, 90);
            }
            else
            {
                p.AddLine(xwr, y, xw, y);
                p.AddLine(xw, y, xw, yr);
            }

            //Right Edge
            p.AddLine(xw, yr, xw, yhr);

            //Bottom Right Corner
            if ((RectangleCorners.BottomRight & corners) == RectangleCorners.BottomRight)
            {
                p.AddArc(xwr2, yhr2, r2, r2, 0, 90);
            }
            else
            {
                p.AddLine(xw, yhr, xw, yh);
                p.AddLine(xw, yh, xwr, yh);
            }

            //Bottom Edge
            p.AddLine(xwr, yh, xr, yh);

            //Bottom Left Corner
            if ((RectangleCorners.BottomLeft & corners) == RectangleCorners.BottomLeft)
            {
                p.AddArc(x, yhr2, r2, r2, 90, 90);
            }
            else
            {
                p.AddLine(xr, yh, x, yh);
                p.AddLine(x, yh, x, yhr);
            }

            //Left Edge
            p.AddLine(x, yhr, x, yr);

            p.CloseFigure();
            return p;
        }

        public static GraphicsPath Create(Rectangle rect, int radius, RectangleCorners c)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius, c); }

        public static GraphicsPath Create(int x, int y, int width, int height, int radius)
        { return Create(x, y, width, height, radius, RectangleCorners.All); }

        public static GraphicsPath Create(Rectangle rect, int radius)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height, radius); }

        public static GraphicsPath Create(int x, int y, int width, int height)
        { return Create(x, y, width, height, 5); }

        public static GraphicsPath Create(Rectangle rect)
        { return Create(rect.X, rect.Y, rect.Width, rect.Height); }
    }
}


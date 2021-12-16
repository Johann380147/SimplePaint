using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class cToolTip : ToolTip
    {
        public cToolTip()
        {
            InitializeComponent();
            this.Popup += new PopupEventHandler(this.OnPopUp);
            this.Draw += new DrawToolTipEventHandler(this.OnDraw);
        }

        private void OnPopUp(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(210, 23);
        }

        private void OnDraw (object sender, DrawToolTipEventArgs e)
        {
            Graphics g = e.Graphics;

            LinearGradientBrush b = new LinearGradientBrush(e.Bounds,
                Color.MidnightBlue, Color.MintCream, 45f);

            g.FillRectangle(b, e.Bounds);

            g.DrawRectangle(new Pen(Brushes.Red, 1), new Rectangle(e.Bounds.X, e.Bounds.Y,
                e.Bounds.Width - 1, e.Bounds.Height - 1));

            g.DrawString(e.ToolTipText, new Font(e.Font, FontStyle.Regular), Brushes.Silver,
                new PointF(e.Bounds.X + 6, e.Bounds.Y + 6));

            g.DrawString(e.ToolTipText, new Font(e.Font, FontStyle.Regular), Brushes.Black,
                new PointF(e.Bounds.X + 5, e.Bounds.Y + 5));

            b.Dispose();
        }
    }
}

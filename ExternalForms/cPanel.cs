using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class cPanel : Panel
    {
        public cPanel()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            base.OnPaint(e);
        }
    }
}

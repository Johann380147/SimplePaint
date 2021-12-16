using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class cPanelBlue : Panel
    {
        public cPanelBlue()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.SteelBlue, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }
    }
}

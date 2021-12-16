using System;
using System.Drawing;
using System.Windows.Forms;
using ExternalForms;

namespace SimplePaint
{
    public partial class ResizeImage : Form
    {
        public Size size { get; set; }
        cToolTip t = new cToolTip();

        public ResizeImage(Size size)
        {
            InitializeComponent();
            this.size = size;
            txtWidth.Text = size.Width.ToString();
            txtHeight.Text = size.Height.ToString();
            txtWidth.Select();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int width = Int32.TryParse(txtWidth.Text, out width) ? width : 0;
            int height = Int32.TryParse(txtHeight.Text, out height) ? height : 0;
            if (width >= 20 && height >= 20)
            {
                size = new Size(width, height);

                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Dispose();
            }
            else if( width < 20)
            {
                t.Show("Width cannot be smaller than 20px!", txtWidth, new Point(txtWidth.Width + 5, 0), 2000);
                txtWidth.Select();
            }
            else
            {
                t.Show("Height cannot be smaller than 20px!", txtHeight, new Point(txtHeight.Width + 5, 0), 2000);
                txtHeight.Select();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        bool moveForm = false;
        Point coordi = new Point();

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            coordi = e.Location;
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
            {
                this.SetDesktopLocation(MousePosition.X - coordi.X, MousePosition.Y - coordi.Y);
            }
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            coordi = new Point(e.Location.X + label4.Left, e.Location.Y + label4.Top);
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
        }

        private void label4_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveForm)
                this.SetDesktopLocation(MousePosition.X - coordi.X, MousePosition.Y - coordi.Y);
        }

        private void ResizeImage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
            {
                this.Dispose();
            }

            if(e.KeyCode == Keys.Enter)
            {
                btnAccept.PerformClick();
            }
        }

    }
}

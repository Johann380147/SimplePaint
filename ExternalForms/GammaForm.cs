using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class GammaForm : Form
    {
        public float red { get; set; }
        public float green { get; set; }
        public float blue { get; set; }
        
        public GammaForm()
        {
            InitializeComponent();
            txtRed.Select();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnCancel.PerformClick();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            red = txtRed.Text.ToSingle();
            green = txtGreen.Text.ToSingle();
            blue = txtBlue.Text.ToSingle();
            if (red >= 0.2f && red <= 5.0f &&
                green >= 0.2f && green <= 5.0f &&
                blue >= 0.2f && blue <= 5.0f)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else if (red < 0.2f || red > 5.0f)
            {
                cToolTip1.Show("Value must be between 0.2 and 5.0!", txtRed,new Point(txtRed.Width + 5, 0), 2000);
                txtRed.Select();
            }
            else if (green < 0.2f || green > 5.0f)
            {
                cToolTip1.Show("Value must be between 0.2 and 5.0!", txtGreen, new Point(txtGreen.Width + 5, 0), 2000);
                txtGreen.Select();
            }
            else
            {
                cToolTip1.Show("Value must be between 0.2 and 5.0!", txtBlue, new Point(txtBlue.Width + 5, 0), 2000);
                txtBlue.Select();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }

        private void txtRed_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1 ||
                e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 ||
                e.KeyCode == Keys.D4 ||
                e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 ||
                e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 ||
                e.KeyCode == Keys.D0 ||
                e.KeyCode == Keys.OemPeriod||
                e.KeyCode == Keys.Back)
            {
                if (e.KeyCode == Keys.OemPeriod)
                {
                    if (txtRed.Text.Contains("."))
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtGreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1 ||
                e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 ||
                e.KeyCode == Keys.D4 ||
                e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 ||
                e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 ||
                e.KeyCode == Keys.D0 ||
                e.KeyCode == Keys.OemPeriod||
                e.KeyCode == Keys.Back)
            {
                if (e.KeyCode == Keys.OemPeriod)
                {
                    if (txtGreen.Text.Contains("."))
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else
            {
                e.SuppressKeyPress = true;
            }
        }

        private void txtBlue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1 ||
                e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 ||
                e.KeyCode == Keys.D4 ||
                e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 ||
                e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 ||
                e.KeyCode == Keys.D0 ||
                e.KeyCode == Keys.OemPeriod||
                e.KeyCode == Keys.Back)
            {
                if (e.KeyCode == Keys.OemPeriod)
                {
                    if (txtBlue.Text.Contains("."))
                    {
                        e.SuppressKeyPress = true;
                    }
                }
            }
            else
            {
                e.SuppressKeyPress = true;
            }
        }

        private void GammaForm_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Alt && e.KeyCode == Keys.F4) || e.KeyCode == Keys.Escape)
            {
                btnCancel.PerformClick();
            }
            if(e.KeyCode == Keys.Enter)
            {
                btnAccept.PerformClick();
            }
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class BrightnessForm : Form
    {
        public float brightValue { get; set; }

        public BrightnessForm()
        {
            InitializeComponent();
            txtBrightness.Select();
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
            brightValue = txtBrightness.Text.ToSingle();

            if(brightValue >= -255.0f && brightValue <= 255.0f)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;   
            }
            else
            {
                cToolTip1.Show("Value must be between -255 and 255!", txtBrightness, new Point(txtBrightness.Width + 5, 0), 2000);
                txtBrightness.Select();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }

        private void txtBrightness_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1 ||e.KeyCode == Keys.NumPad1||
                e.KeyCode == Keys.D2 ||e.KeyCode == Keys.NumPad2||
                e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3 ||
                e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4 ||
                e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5 ||
                e.KeyCode == Keys.D6 || e.KeyCode == Keys.NumPad6 ||
                e.KeyCode == Keys.D7 || e.KeyCode == Keys.NumPad7 ||
                e.KeyCode == Keys.D8 || e.KeyCode == Keys.NumPad8 ||
                e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad9 ||
                e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0 ||
                e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract||
                e.KeyCode == Keys.OemPeriod||e.KeyCode == Keys.Decimal||
                e.KeyCode == Keys.Back)
            {
                if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal)
                {
                    if (txtBrightness.Text.Contains("."))
                    {
                        e.SuppressKeyPress = true;
                    }
                }
                if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
                {
                    if (txtBrightness.Text.Contains("-"))
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

        private void BrightnessForm_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Alt && e.KeyCode == Keys.F4) || e.KeyCode == Keys.Escape)
            {
                btnCancel.PerformClick();
            }
            if (e.KeyCode == Keys.Enter)
            {
                btnAccept.PerformClick();
            }
        }
    }
}

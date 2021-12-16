using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;
using ExternalForms;

namespace SimplePaint
{
    public partial class CustomBrush : Form
    {
        public HatchStyle style { get; set; }
        public Color foreHatchColor { get; set; }
        public Color backHatchColor { get; set; }

        HatchBrush hb;
        Bitmap bmp;
        ToolTip tp = new ToolTip();


        public CustomBrush(HatchStyle style, Color foreHatchColor, Color backHatchColor)
        {
            InitializeComponent();
            
            this.style = style;
            this.foreHatchColor = foreHatchColor;
            this.backHatchColor = backHatchColor;
            hb = new HatchBrush(style, foreHatchColor, backHatchColor);

            lblFore.BackColor = foreHatchColor;
            lblBack.BackColor = backHatchColor;

            txtForeColor.Text = lblFore.BackColor.ToString();
            txtBackColor.Text = lblBack.BackColor.ToString();

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            var autoComplete = new AutoCompleteStringCollection();
            foreach (string styleName in Enum.GetNames(typeof(HatchStyle)))
            {
                lstStyle.Items.Add(styleName);
                autoComplete.Add(styleName);
            }
            lstStyle.SelectedItem = style.ToString();
            txtStyle.Text = lstStyle.SelectedItem.ToString();
            txtStyle.AutoCompleteCustomSource = autoComplete;

            tp.SetToolTip(lblFore, "Click to select fore color");
            tp.SetToolTip(lblBack, "Click to select back color");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lblFore_Click(object sender, EventArgs e)
        {
            arrow.Visible = false;
            cColorDialog c = new cColorDialog();
            
            if(c.ShowDialog() == DialogResult.OK)
            {
                lblFore.BackColor = c.ColorSelected;
                txtForeColor.Text = c.ColorSelected.ToString();
                foreHatchColor = c.ColorSelected;

                hb.Dispose();
                hb = new HatchBrush(style, foreHatchColor, backHatchColor);

                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
                }
                pictureBox1.Invalidate();
            }

        }

        private void lblBack_Click(object sender, EventArgs e)
        {
            arrow2.Visible = false;

            cColorDialog c = new cColorDialog();

            if (c.ShowDialog() == DialogResult.OK)
            {
                lblBack.BackColor = c.ColorSelected;
                txtBackColor.Text = c.ColorSelected.ToString();
                backHatchColor = c.ColorSelected;

                hb.Dispose();
                hb = new HatchBrush(style, foreHatchColor, backHatchColor);

                using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                {
                    g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
                }
                pictureBox1.Invalidate();
            }
        }

        private void lstStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtStyle.Text = lstStyle.SelectedItem.ToString();
            style = (HatchStyle)Enum.Parse(typeof(HatchStyle),
                             lstStyle.SelectedItem.ToString(), true);

            hb.Dispose();
            hb = new HatchBrush(style, foreHatchColor, backHatchColor);

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))
            {
                g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            pictureBox1.Invalidate();
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

        private void txtForeColor_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            timer1.Enabled = true;
            time = 0;
            arrow.Visible = true;
            arrow2.Visible = false;
        }

        private void txtBackColor_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            timer1.Enabled = true;
            time = 0;
            arrow2.Visible = true;
            arrow.Visible = false;
        }


        bool up = false;
        bool down = true;
        bool upBack = false;
        bool downBack = true;
        int offset = 0;
        int offsetBack = 0;
        int time = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(time > 3000)
            {
                timer1.Enabled = false;
                arrow.Visible = false;
                arrow2.Visible = false;
                time = 0;
            }
            if (arrow.Visible)
            {
                if (arrow.Location.Y <= 28)
                {
                    down = true;
                    up = false;
                }
                else if (arrow.Location.Y > 37)
                {
                    up = true;
                    down = false;
                }

                if (down)
                {
                    offset += 1;
                }
                else if (up)
                {
                    offset -= 1;
                }
            }
            if (arrow2.Visible)
            {
                if (arrow2.Location.Y <= 111)
                {
                    downBack = true;
                    upBack = false;
                }
                else if (arrow2.Location.Y > 120)
                {
                    upBack = true;
                    downBack = false;
                }

                if (downBack)
                {
                    offsetBack += 1;
                }
                else if (upBack)
                {
                    offsetBack -= 1;
                }
            }
            time += 50;
            if (arrow.Visible)
            {
                arrow.Location = new Point(292, 28 + offset);
            }

            if (arrow2.Visible)
            {
                arrow2.Location = new Point(292, 111 + offsetBack);
            }
        }

        private void txtStyle_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStyle.Text)) return;

            HatchStyle temp;

            if (Enum.TryParse(txtStyle.Text, true, out temp))
            {
                if (Enum.IsDefined(typeof(HatchStyle), temp))
                {
                    style = temp;
                    txtStyle.Text = style.ToString();

                    hb.Dispose();
                    hb = new HatchBrush(style, foreHatchColor, backHatchColor);

                    using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                    {
                        g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
                        pictureBox1.Invalidate();
                    }
                }
            }
        }

        private void txtStyle_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
            {
                if (string.IsNullOrWhiteSpace(txtStyle.Text)) return;

                HatchStyle temp;

                if (Enum.TryParse(txtStyle.Text, true, out temp))
                {
                    if (Enum.IsDefined(typeof(HatchStyle), temp))
                    {
                        style = temp;
                        txtStyle.Text = style.ToString();

                        hb.Dispose();
                        hb = new HatchBrush(style, foreHatchColor, backHatchColor);

                        using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                        {
                            g.FillRectangle(hb, new Rectangle(0, 0, bmp.Width, bmp.Height));
                            pictureBox1.Invalidate();
                        }
                    }
                }
            }
        }

        private void CustomBrush_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape || (e.Alt && e.KeyCode == Keys.F4))
                this.Dispose();
            if(e.KeyCode == Keys.Enter)
            {
                btnAccept.PerformClick();
            }
        }

    }
}

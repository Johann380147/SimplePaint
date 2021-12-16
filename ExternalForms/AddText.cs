using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class AddText : Form
    {
        public FontFamily fontFamily = new FontFamily("Calibri");
        public FontStyle fontStyle = FontStyle.Regular;
        string[] arraySize = {"8","9","10","11","12","14","16","18","20",
                                 "22","24","26","28","36","48","72"};
        public int size = 8;
        public Color color = Color.Black;


        public AddText()
        {
            InitializeComponent();
            comboBoxSize.LostFocus += (sender, e) =>
            {
                int y = Int32.TryParse(comboBoxSize.Text, out y) ? y : 0;

                if (y >= 8 && y <= 72)
                {
                    size = y;
                    updatePicturePreview();
                }
                else comboBoxSize.Text = size.ToString();

            };

            var sourceFont = new AutoCompleteStringCollection();
            var sourceStyle = new AutoCompleteStringCollection();

            foreach (FontFamily f in FontFamily.Families)
            {
                comboBoxFont.Items.Add(f.Name);
                sourceFont.Add(f.Name);
            }
            comboBoxFont.AutoCompleteCustomSource = sourceFont;
            comboBoxFont.Text = fontFamily.Name;

            foreach (string style in Enum.GetNames(typeof(FontStyle)))
            {
                comboBoxStyle.Items.Add(style);
                sourceStyle.Add(style);
            }
            comboBoxStyle.AutoCompleteCustomSource = sourceStyle;
            comboBoxStyle.Text = fontStyle.ToString();

            foreach (string s in arraySize)
            {
                comboBoxSize.Items.Add(s);
            }
            comboBoxSize.Text = size.ToString();
            
        }


        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (txtText.Text != null)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Hide();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontFamily = new FontFamily(comboBoxFont.SelectedItem.ToString());

            updatePicturePreview();
        }

        private void comboBoxStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), comboBoxStyle.Text);

            updatePicturePreview();
        }

        private void comboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            size = Convert.ToInt32(comboBoxSize.Text);

            updatePicturePreview();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            cColorDialog c = new cColorDialog();
            
            if(c.ShowDialog() == DialogResult.OK)
            {
                color = c.ColorSelected;
                btnColor.BackColor = color;
                txtText.ForeColor = color;

                updatePicturePreview();
            }
        }

        private void updatePicturePreview()
        {
            txtText.Font = new Font(fontFamily, size, fontStyle);
            txtPreview.Font = new Font(fontFamily, 12f, fontStyle);
            txtPreview.ForeColor = color;
        }

        private void comboBoxSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                int y = Int32.TryParse(comboBoxSize.Text, out y) ? y : 0;

                if (y >= 8 && y <= 72)
                {
                    size = y;
                    updatePicturePreview();
                }
                else comboBoxSize.Text = size.ToString();

                e.SuppressKeyPress = true;
            }
        }

        public string GetText()
        {
            return txtText.Text;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
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

    }
}

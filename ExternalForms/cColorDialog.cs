using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class cColorDialog : Form
    {
        private Color colorSelected;
        public Color ColorSelected
        {
            get { return colorSelected; }
            private set
            {
                colorSelected = value;
            }
        }

        public cColorDialog()
        {
            InitializeComponent();
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

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            moveForm = true;
            coordi = new Point(e.Location.X + lblTitle.Left, e.Location.Y + lblTitle.Top);
        }

        private void lblTitle_MouseUp(object sender, MouseEventArgs e)
        {
            moveForm = false;
        }

        private void lblTitle_MouseMove(object sender, MouseEventArgs e)
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
            DialogResult = System.Windows.Forms.DialogResult.OK;
            ColorPicker p = (ColorPicker)elementHost1.Child;
            this.ColorSelected = Color.FromArgb(p.SelectedColor.A, p.SelectedColor.R, p.SelectedColor.G, p.SelectedColor.B);
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Dispose();
        }

    }
}

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace ExternalForms
{
    public partial class AboutPaint : Form
    {
        public AboutPaint()
        {
            InitializeComponent();
            lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
    }
}

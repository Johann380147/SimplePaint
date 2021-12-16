using System;
using System.Reflection;
using System.Windows.Forms;

namespace ExternalForms
{
    public partial class StartUp : Form
    {
        cColorDialog c = new cColorDialog();

        public StartUp()
        {
            InitializeComponent();
            lblDescription.Text = String.Format("This product is licensed to:\n{0}", Environment.UserName);
            lblVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            IntPtr dummy = c.Handle;
        }

        
        private void StartUp_Shown(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Interval = 2500;
            timer.Start();
            timer.Tick += (s, a) => this.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(this.Opacity < 1.0)
            {
                this.Opacity += 0.1;
            }
        }
    }
}

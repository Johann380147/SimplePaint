using System;
using System.IO;
using System.Windows.Forms;

namespace SimplePaint
{
    public partial class BasicSave : Form
    {
        public BasicSave()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public static DialogResult Show(string path)
        {
            using (BasicSave sav = new BasicSave())
            {
                if (!string.IsNullOrWhiteSpace(path))
                    sav.label1.Text = String.Format("Do you want to save changes to {0}?", Path.GetFileNameWithoutExtension(path));
                return sav.ShowDialog();
            }
        }
    }
}

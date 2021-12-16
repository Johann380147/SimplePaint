namespace ExternalForms
{
    partial class StartUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartUp));
            this.lblVersion = new System.Windows.Forms.Label();
            this.labelCopyrights = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer();
            this.cPanelBlue1 = new ExternalForms.cPanelBlue();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblVersion.Location = new System.Drawing.Point(94, 94);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(66, 13);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "Version 1.00";
            // 
            // labelCopyrights
            // 
            this.labelCopyrights.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCopyrights.AutoSize = true;
            this.labelCopyrights.BackColor = System.Drawing.Color.Transparent;
            this.labelCopyrights.Font = new System.Drawing.Font("Corbel", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCopyrights.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.labelCopyrights.Location = new System.Drawing.Point(83, 311);
            this.labelCopyrights.Name = "labelCopyrights";
            this.labelCopyrights.Size = new System.Drawing.Size(218, 75);
            this.labelCopyrights.TabIndex = 4;
            this.labelCopyrights.Text = "This program along with any source\r\ncode and files are licensed under\r\nThe Code P" +
    "roject Open License (CPOL).\r\n© 2015 Simple Paint Inc.\r\nAll rights reserved.";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Gabriola", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Image = global::ExternalForms.Properties.Resources.paintIcon;
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(325, 85);
            this.label1.TabIndex = 2;
            this.label1.Text = "SIMPLE PAINT";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescription.AutoSize = true;
            this.lblDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblDescription.Font = new System.Drawing.Font("Corbel", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblDescription.Location = new System.Drawing.Point(94, 187);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(150, 30);
            this.lblDescription.TabIndex = 4;
            this.lblDescription.Text = "This product is licensed to:\r\nAbc";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 25;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cPanelBlue1
            // 
            this.cPanelBlue1.BackColor = System.Drawing.Color.Transparent;
            this.cPanelBlue1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cPanelBlue1.Location = new System.Drawing.Point(0, 0);
            this.cPanelBlue1.Name = "cPanelBlue1";
            this.cPanelBlue1.Size = new System.Drawing.Size(355, 395);
            this.cPanelBlue1.TabIndex = 5;
            // 
            // StartUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(355, 395);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.labelCopyrights);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cPanelBlue1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StartUp";
            this.Opacity = 0.05D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.StartUp_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label labelCopyrights;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Timer timer1;
        private cPanelBlue cPanelBlue1;
    }
}
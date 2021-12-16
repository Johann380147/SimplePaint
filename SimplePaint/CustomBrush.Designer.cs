namespace SimplePaint
{
    partial class CustomBrush
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomBrush));
            this.txtStyle = new System.Windows.Forms.TextBox();
            this.lblStyle = new System.Windows.Forms.Label();
            this.lblForeColor = new System.Windows.Forms.Label();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.lblFore = new System.Windows.Forms.Label();
            this.txtForeColor = new System.Windows.Forms.TextBox();
            this.lblBack = new System.Windows.Forms.Label();
            this.lstStyle = new System.Windows.Forms.ListBox();
            this.txtBackColor = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cPanel1 = new ExternalForms.cPanel();
            this.btnCancel = new ExternalForms.cButton();
            this.btnAccept = new ExternalForms.cButton();
            this.arrow2 = new System.Windows.Forms.PictureBox();
            this.arrow = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelTop.SuspendLayout();
            this.cPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.arrow2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrow)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStyle
            // 
            this.txtStyle.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtStyle.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtStyle.Location = new System.Drawing.Point(16, 66);
            this.txtStyle.Name = "txtStyle";
            this.txtStyle.Size = new System.Drawing.Size(130, 20);
            this.txtStyle.TabIndex = 0;
            this.txtStyle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtStyle_KeyDown);
            this.txtStyle.Leave += new System.EventHandler(this.txtStyle_Leave);
            // 
            // lblStyle
            // 
            this.lblStyle.AutoSize = true;
            this.lblStyle.Font = new System.Drawing.Font("Buxton Sketch", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStyle.Location = new System.Drawing.Point(13, 49);
            this.lblStyle.Name = "lblStyle";
            this.lblStyle.Size = new System.Drawing.Size(33, 16);
            this.lblStyle.TabIndex = 3;
            this.lblStyle.Text = "Style";
            // 
            // lblForeColor
            // 
            this.lblForeColor.AutoSize = true;
            this.lblForeColor.Font = new System.Drawing.Font("Buxton Sketch", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblForeColor.Location = new System.Drawing.Point(162, 50);
            this.lblForeColor.Name = "lblForeColor";
            this.lblForeColor.Size = new System.Drawing.Size(57, 16);
            this.lblForeColor.TabIndex = 3;
            this.lblForeColor.Text = "Fore Color";
            // 
            // lblBackColor
            // 
            this.lblBackColor.AutoSize = true;
            this.lblBackColor.Font = new System.Drawing.Font("Buxton Sketch", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackColor.Location = new System.Drawing.Point(162, 131);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(60, 16);
            this.lblBackColor.TabIndex = 3;
            this.lblBackColor.Text = "Back Color";
            // 
            // lblFore
            // 
            this.lblFore.BackColor = System.Drawing.Color.White;
            this.lblFore.ForeColor = System.Drawing.Color.Black;
            this.lblFore.Location = new System.Drawing.Point(285, 66);
            this.lblFore.Name = "lblFore";
            this.lblFore.Size = new System.Drawing.Size(41, 40);
            this.lblFore.TabIndex = 4;
            this.lblFore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFore.Click += new System.EventHandler(this.lblFore_Click);
            // 
            // txtForeColor
            // 
            this.txtForeColor.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtForeColor.Location = new System.Drawing.Point(165, 66);
            this.txtForeColor.Multiline = true;
            this.txtForeColor.Name = "txtForeColor";
            this.txtForeColor.Size = new System.Drawing.Size(97, 40);
            this.txtForeColor.TabIndex = 1;
            this.txtForeColor.TabStop = false;
            this.txtForeColor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtForeColor_KeyDown);
            // 
            // lblBack
            // 
            this.lblBack.BackColor = System.Drawing.Color.Black;
            this.lblBack.Location = new System.Drawing.Point(285, 147);
            this.lblBack.Name = "lblBack";
            this.lblBack.Size = new System.Drawing.Size(41, 40);
            this.lblBack.TabIndex = 4;
            this.lblBack.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBack.Click += new System.EventHandler(this.lblBack_Click);
            // 
            // lstStyle
            // 
            this.lstStyle.FormattingEnabled = true;
            this.lstStyle.Location = new System.Drawing.Point(16, 92);
            this.lstStyle.Name = "lstStyle";
            this.lstStyle.Size = new System.Drawing.Size(130, 95);
            this.lstStyle.TabIndex = 1;
            this.lstStyle.SelectedIndexChanged += new System.EventHandler(this.lstStyle_SelectedIndexChanged);
            // 
            // txtBackColor
            // 
            this.txtBackColor.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtBackColor.Location = new System.Drawing.Point(165, 147);
            this.txtBackColor.Multiline = true;
            this.txtBackColor.Name = "txtBackColor";
            this.txtBackColor.Size = new System.Drawing.Size(97, 40);
            this.txtBackColor.TabIndex = 2;
            this.txtBackColor.TabStop = false;
            this.txtBackColor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBackColor_KeyDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Font = new System.Drawing.Font("Buxton Sketch", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 198);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(105, 90);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Preview";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(19, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(68, 56);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.panelTop.Controls.Add(this.btnClose);
            this.panelTop.Controls.Add(this.label4);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(343, 25);
            this.panelTop.TabIndex = 10;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.LightGray;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(320, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(23, 21);
            this.btnClose.TabIndex = 20;
            this.btnClose.TabStop = false;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Buxton Sketch", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(120, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 20);
            this.label4.TabIndex = 21;
            this.label4.Text = "Custom Brush";
            this.label4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label4_MouseDown);
            this.label4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label4_MouseMove);
            this.label4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label4_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Interval = 35;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cPanel1
            // 
            this.cPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.cPanel1.Controls.Add(this.btnCancel);
            this.cPanel1.Controls.Add(this.btnAccept);
            this.cPanel1.Controls.Add(this.arrow2);
            this.cPanel1.Controls.Add(this.arrow);
            this.cPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cPanel1.Location = new System.Drawing.Point(0, 0);
            this.cPanel1.Name = "cPanel1";
            this.cPanel1.Size = new System.Drawing.Size(343, 313);
            this.cPanel1.TabIndex = 12;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Buxton Sketch", 10F);
            this.btnCancel.Location = new System.Drawing.Point(246, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Font = new System.Drawing.Font("Buxton Sketch", 10F);
            this.btnAccept.Location = new System.Drawing.Point(156, 254);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(80, 34);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "Accept";
            this.btnAccept.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // arrow2
            // 
            this.arrow2.Image = global::SimplePaint.Properties.Resources.arrowIndicatorDown;
            this.arrow2.Location = new System.Drawing.Point(292, 109);
            this.arrow2.Name = "arrow2";
            this.arrow2.Size = new System.Drawing.Size(24, 24);
            this.arrow2.TabIndex = 0;
            this.arrow2.TabStop = false;
            this.arrow2.Visible = false;
            // 
            // arrow
            // 
            this.arrow.Image = global::SimplePaint.Properties.Resources.arrowIndicatorDown;
            this.arrow.Location = new System.Drawing.Point(292, 28);
            this.arrow.Name = "arrow";
            this.arrow.Size = new System.Drawing.Size(24, 24);
            this.arrow.TabIndex = 0;
            this.arrow.TabStop = false;
            this.arrow.Visible = false;
            // 
            // CustomBrush
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(343, 313);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lstStyle);
            this.Controls.Add(this.txtBackColor);
            this.Controls.Add(this.txtForeColor);
            this.Controls.Add(this.lblBack);
            this.Controls.Add(this.lblFore);
            this.Controls.Add(this.lblBackColor);
            this.Controls.Add(this.lblForeColor);
            this.Controls.Add(this.lblStyle);
            this.Controls.Add(this.txtStyle);
            this.Controls.Add(this.cPanel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomBrush";
            this.Opacity = 0.95D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Custom Brush";
            this.TopMost = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomBrush_KeyDown);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.cPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.arrow2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arrow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStyle;
        private System.Windows.Forms.Label lblStyle;
        private System.Windows.Forms.Label lblForeColor;
        private System.Windows.Forms.Label lblBackColor;
        private System.Windows.Forms.Label lblFore;
        private System.Windows.Forms.TextBox txtForeColor;
        private System.Windows.Forms.Label lblBack;
        private System.Windows.Forms.ListBox lstStyle;
        private System.Windows.Forms.TextBox txtBackColor;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private ExternalForms.cPanel cPanel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox arrow;
        private System.Windows.Forms.PictureBox arrow2;
        private ExternalForms.cButton btnAccept;
        private ExternalForms.cButton btnCancel;
    }
}
namespace Nelian
{
    partial class Splash
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2ProgressBar1 = new Guna.UI2.WinForms.Guna2ProgressBar();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // guna2ProgressBar1
            // 
            guna2ProgressBar1.Anchor = AnchorStyles.Bottom;
            guna2ProgressBar1.BorderRadius = 14;
            guna2ProgressBar1.CustomizableEdges = customizableEdges1;
            guna2ProgressBar1.Location = new Point(252, 466);
            guna2ProgressBar1.Name = "guna2ProgressBar1";
            guna2ProgressBar1.ProgressColor = Color.FromArgb(0, 170, 255);
            guna2ProgressBar1.ProgressColor2 = Color.FromArgb(0, 170, 255);
            guna2ProgressBar1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2ProgressBar1.Size = new Size(364, 26);
            guna2ProgressBar1.TabIndex = 0;
            guna2ProgressBar1.Text = "guna2ProgressBar1";
            guna2ProgressBar1.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            guna2ProgressBar1.ValueChanged += guna2ProgressBar1_ValueChanged_1;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources._8383_07d35a0f7bbcb67fee06f4a95c45a75d_30_09_2025_22_12_37;
            pictureBox1.Location = new Point(320, 110);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(233, 233);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label1.Location = new Point(340, 432);
            label1.Name = "label1";
            label1.Size = new Size(174, 21);
            label1.TabIndex = 3;
            label1.Text = "Checking for Updates";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Splash
            // 
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(guna2ProgressBar1);
            Name = "Splash";
            Size = new Size(884, 521);
            Load += Splash_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2ProgressBar guna2ProgressBar1;
        private PictureBox pictureBox1;
        private Label label1;
    }
}

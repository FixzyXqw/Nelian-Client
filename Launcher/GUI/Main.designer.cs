namespace Nelian
{
    partial class Main
    {
        private System.ComponentModel.IContainer components = null;
        // Ayarlar butonunu da daha şık animasyonlar için Guna2Button'a çevirdik
        private Guna.UI2.WinForms.Guna2Button settingsbtn;
        private Guna.UI2.WinForms.Guna2GradientButton guna2GradientButton1;
        private Guna.UI2.WinForms.Guna2GradientButton guna2GradientButton2;
        private Guna.UI2.WinForms.Guna2PictureBox pictureBox3; // PictureBox'a yumuşak kenar vermek için Guna2 yaptık
        private System.Windows.Forms.Label sessionname;
        private System.Windows.Forms.Label sessionuuid;
        private System.Windows.Forms.Label informationlabel;
        private Guna.UI2.WinForms.Guna2Panel userProfilePanel; // Kullanıcı bilgilerini bir arada tutan şık bir panel

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            settingsbtn = new Guna.UI2.WinForms.Guna2Button();
            guna2GradientButton1 = new Guna.UI2.WinForms.Guna2GradientButton();
            guna2GradientButton2 = new Guna.UI2.WinForms.Guna2GradientButton();
            pictureBox3 = new Guna.UI2.WinForms.Guna2PictureBox();
            sessionname = new Label();
            sessionuuid = new Label();
            informationlabel = new Label();
            userProfilePanel = new Guna.UI2.WinForms.Guna2Panel();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            userProfilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // settingsbtn
            // 
            settingsbtn.Anchor = AnchorStyles.Bottom;
            settingsbtn.Animated = true;
            settingsbtn.BorderRadius = 12;
            settingsbtn.CustomizableEdges = customizableEdges1;
            settingsbtn.DisabledState.BorderColor = Color.DarkGray;
            settingsbtn.DisabledState.CustomBorderColor = Color.DarkGray;
            settingsbtn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            settingsbtn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            settingsbtn.FillColor = Color.FromArgb(28, 35, 47);
            settingsbtn.Font = new Font("Segoe UI", 9F);
            settingsbtn.ForeColor = Color.White;
            settingsbtn.HoverState.FillColor = Color.FromArgb(40, 50, 68);
            settingsbtn.Location = new Point(820, 397);
            settingsbtn.Margin = new Padding(4, 3, 4, 3);
            settingsbtn.Name = "settingsbtn";
            settingsbtn.ShadowDecoration.CustomizableEdges = customizableEdges2;
            settingsbtn.Size = new Size(60, 52);
            settingsbtn.TabIndex = 2;
            settingsbtn.Click += settingsbtn_Click;
            // 
            // guna2GradientButton1
            // 
            guna2GradientButton1.Anchor = AnchorStyles.Bottom;
            guna2GradientButton1.Animated = true;
            guna2GradientButton1.BackColor = Color.Transparent;
            guna2GradientButton1.BorderRadius = 15;
            guna2GradientButton1.CustomizableEdges = customizableEdges3;
            guna2GradientButton1.DisabledState.BorderColor = Color.DarkGray;
            guna2GradientButton1.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2GradientButton1.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2GradientButton1.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            guna2GradientButton1.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2GradientButton1.Enabled = false;
            guna2GradientButton1.FillColor = Color.FromArgb(0, 242, 254);
            guna2GradientButton1.FillColor2 = Color.FromArgb(79, 172, 254);
            guna2GradientButton1.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
            guna2GradientButton1.ForeColor = Color.White;
            guna2GradientButton1.HoverState.FillColor = Color.FromArgb(79, 172, 254);
            guna2GradientButton1.HoverState.FillColor2 = Color.FromArgb(0, 242, 254);
            guna2GradientButton1.Location = new Point(410, 455);
            guna2GradientButton1.Margin = new Padding(4, 3, 4, 3);
            guna2GradientButton1.Name = "guna2GradientButton1";
            guna2GradientButton1.ShadowDecoration.BorderRadius = 15;
            guna2GradientButton1.ShadowDecoration.Color = Color.FromArgb(0, 242, 254);
            guna2GradientButton1.ShadowDecoration.CustomizableEdges = customizableEdges4;
            guna2GradientButton1.ShadowDecoration.Depth = 15;
            guna2GradientButton1.ShadowDecoration.Enabled = true;
            guna2GradientButton1.Size = new Size(288, 52);
            guna2GradientButton1.TabIndex = 1;
            guna2GradientButton1.Text = "Launch";
            guna2GradientButton1.Click += guna2GradientButton1_Click_1;
            // 
            // guna2GradientButton2
            // 
            guna2GradientButton2.Anchor = AnchorStyles.Bottom;
            guna2GradientButton2.Animated = true;
            guna2GradientButton2.BorderRadius = 15;
            guna2GradientButton2.CustomizableEdges = customizableEdges5;
            guna2GradientButton2.DisabledState.BorderColor = Color.DarkGray;
            guna2GradientButton2.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2GradientButton2.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2GradientButton2.DisabledState.FillColor2 = Color.FromArgb(169, 169, 169);
            guna2GradientButton2.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2GradientButton2.FillColor = Color.FromArgb(43, 50, 65);
            guna2GradientButton2.FillColor2 = Color.FromArgb(28, 35, 47);
            guna2GradientButton2.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            guna2GradientButton2.ForeColor = Color.FromArgb(224, 224, 224);
            guna2GradientButton2.HoverState.FillColor = Color.FromArgb(53, 62, 81);
            guna2GradientButton2.HoverState.FillColor2 = Color.FromArgb(38, 47, 63);
            guna2GradientButton2.HoverState.ForeColor = Color.White;
            guna2GradientButton2.Location = new Point(710, 455);
            guna2GradientButton2.Margin = new Padding(4, 3, 4, 3);
            guna2GradientButton2.Name = "guna2GradientButton2";
            guna2GradientButton2.ShadowDecoration.CustomizableEdges = customizableEdges6;
            guna2GradientButton2.Size = new Size(170, 52);
            guna2GradientButton2.TabIndex = 2;
            guna2GradientButton2.Text = "Accounts";
            guna2GradientButton2.Click += guna2GradientButton2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Transparent;
            pictureBox3.BorderRadius = 10;
            pictureBox3.CustomizableEdges = customizableEdges7;
            pictureBox3.ImageRotate = 0F;
            pictureBox3.Location = new Point(10, 10);
            pictureBox3.Margin = new Padding(4, 3, 4, 3);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.ShadowDecoration.CustomizableEdges = customizableEdges8;
            pictureBox3.Size = new Size(46, 46);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 3;
            pictureBox3.TabStop = false;
            // 
            // sessionname
            // 
            sessionname.AutoSize = true;
            sessionname.BackColor = Color.Transparent;
            sessionname.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            sessionname.ForeColor = Color.White;
            sessionname.Location = new Point(64, 12);
            sessionname.Margin = new Padding(4, 0, 4, 0);
            sessionname.Name = "sessionname";
            sessionname.Size = new Size(78, 20);
            sessionname.TabIndex = 4;
            sessionname.Text = "Username";
            // 
            // sessionuuid
            // 
            sessionuuid.AutoSize = true;
            sessionuuid.BackColor = Color.Transparent;
            sessionuuid.Font = new Font("Segoe UI", 8F);
            sessionuuid.ForeColor = Color.FromArgb(139, 148, 158);
            sessionuuid.Location = new Point(64, 35);
            sessionuuid.Margin = new Padding(4, 0, 4, 0);
            sessionuuid.Name = "sessionuuid";
            sessionuuid.Size = new Size(34, 13);
            sessionuuid.TabIndex = 5;
            sessionuuid.Text = "UUID";
            // 
            // informationlabel
            // 
            informationlabel.Anchor = AnchorStyles.Bottom;
            informationlabel.AutoSize = true;
            informationlabel.BackColor = Color.Transparent;
            informationlabel.Font = new Font("Segoe UI", 9F);
            informationlabel.ForeColor = Color.FromArgb(139, 148, 158);
            informationlabel.Location = new Point(413, 427);
            informationlabel.Margin = new Padding(4, 0, 4, 0);
            informationlabel.Name = "informationlabel";
            informationlabel.Size = new Size(98, 15);
            informationlabel.TabIndex = 6;
            informationlabel.Text = "Ready to launch..";
            // 
            // userProfilePanel
            // 
            userProfilePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            userProfilePanel.BorderRadius = 16;
            userProfilePanel.Controls.Add(pictureBox3);
            userProfilePanel.Controls.Add(sessionname);
            userProfilePanel.Controls.Add(sessionuuid);
            userProfilePanel.CustomizableEdges = customizableEdges9;
            userProfilePanel.FillColor = Color.FromArgb(22, 27, 37);
            userProfilePanel.Location = new Point(15, 445);
            userProfilePanel.Name = "userProfilePanel";
            userProfilePanel.ShadowDecoration.CustomizableEdges = customizableEdges10;
            userProfilePanel.Size = new Size(373, 66);
            userProfilePanel.TabIndex = 7;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.WhatsApp_Image_2026_07_15_at_00_03_01_removebg_preview;
            pictureBox4.Location = new Point(295, 13);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(253, 128);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 8;
            pictureBox4.TabStop = false;
            // 
            // Main
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(11, 14, 20);
            Controls.Add(pictureBox4);
            Controls.Add(userProfilePanel);
            Controls.Add(informationlabel);
            Controls.Add(settingsbtn);
            Controls.Add(guna2GradientButton2);
            Controls.Add(guna2GradientButton1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Main";
            Size = new Size(884, 521);
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            userProfilePanel.ResumeLayout(false);
            userProfilePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private PictureBox pictureBox4;
    }
}

namespace Nelian
{
    partial class AccountControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbAvatar;
        private System.Windows.Forms.Label lbUsername;
        private System.Windows.Forms.Label lbIdentifier;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel panelButtons;

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
            panel1 = new Panel();
            pbAvatar = new PictureBox();
            lbUsername = new Label();
            lbIdentifier = new Label();
            panelButtons = new Panel();
            btnLogin = new Button();
            btnRemove = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).BeginInit();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(45, 45, 45);
            panel1.Controls.Add(pbAvatar);
            panel1.Controls.Add(lbUsername);
            panel1.Controls.Add(lbIdentifier);
            panel1.Controls.Add(panelButtons);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(292, 231);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // pbAvatar
            // 
            pbAvatar.BackColor = Color.Transparent;
            pbAvatar.Location = new Point(88, 12);
            pbAvatar.Margin = new Padding(4, 3, 4, 3);
            pbAvatar.Name = "pbAvatar";
            pbAvatar.Size = new Size(117, 115);
            pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            pbAvatar.TabIndex = 0;
            pbAvatar.TabStop = false;
            // 
            // lbUsername
            // 
            lbUsername.AutoSize = true;
            lbUsername.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbUsername.ForeColor = Color.White;
            lbUsername.Location = new Point(70, 130);
            lbUsername.Margin = new Padding(4, 0, 4, 0);
            lbUsername.Name = "lbUsername";
            lbUsername.Size = new Size(87, 21);
            lbUsername.TabIndex = 1;
            lbUsername.Text = "Username";
            // 
            // lbIdentifier
            // 
            lbIdentifier.AutoSize = true;
            lbIdentifier.Font = new Font("Segoe UI", 8F);
            lbIdentifier.ForeColor = Color.FromArgb(180, 180, 180);
            lbIdentifier.Location = new Point(82, 155);
            lbIdentifier.Margin = new Padding(4, 0, 4, 0);
            lbIdentifier.Name = "lbIdentifier";
            lbIdentifier.Size = new Size(54, 13);
            lbIdentifier.TabIndex = 2;
            lbIdentifier.Text = "Identifier";
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnLogin);
            panelButtons.Controls.Add(btnRemove);
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Location = new Point(0, 185);
            panelButtons.Margin = new Padding(4, 3, 4, 3);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(292, 46);
            panelButtons.TabIndex = 3;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.FromArgb(52, 152, 219);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(12, 6);
            btnLogin.Margin = new Padding(4, 3, 4, 3);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(122, 35);
            btnLogin.TabIndex = 0;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click_1;
            // 
            // btnRemove
            // 
            btnRemove.BackColor = Color.FromArgb(231, 76, 60);
            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.FlatStyle = FlatStyle.Flat;
            btnRemove.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemove.ForeColor = Color.White;
            btnRemove.Location = new Point(152, 6);
            btnRemove.Margin = new Padding(4, 3, 4, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(122, 35);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = false;
            btnRemove.Click += btnRemove_Click;
            // 
            // AccountControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(30, 30, 30);
            Controls.Add(panel1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AccountControl";
            Size = new Size(292, 231);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbAvatar).EndInit();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}

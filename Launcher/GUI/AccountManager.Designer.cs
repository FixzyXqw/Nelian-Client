namespace Nelian
{
    partial class AccountManager
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowAccounts;

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
            flowAccounts = new FlowLayoutPanel();
            panel1 = new Panel();
            button4 = new Button();
            label3 = new Label();
            guna2TextBox1 = new Guna.UI2.WinForms.Guna2TextBox();
            label2 = new Label();
            button3 = new Button();
            label1 = new Label();
            button2 = new Button();
            button1 = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowAccounts
            // 
            flowAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowAccounts.AutoScroll = true;
            flowAccounts.BackColor = Color.FromArgb(20, 20, 20);
            flowAccounts.Location = new Point(4, 6);
            flowAccounts.Margin = new Padding(4, 3, 4, 3);
            flowAccounts.Name = "flowAccounts";
            flowAccounts.Padding = new Padding(12);
            flowAccounts.Size = new Size(876, 450);
            flowAccounts.TabIndex = 1;
            flowAccounts.Paint += flowAccounts_Paint;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(30, 30, 30);
            panel1.Controls.Add(button4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(guna2TextBox1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(button2);
            panel1.Location = new Point(182, 93);
            panel1.Name = "panel1";
            panel1.Size = new Size(559, 285);
            panel1.TabIndex = 0;
            panel1.Visible = false;
            panel1.Paint += panel1_Paint;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.BackColor = Color.FromArgb(64, 64, 64);
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button4.ForeColor = Color.White;
            button4.Location = new Point(508, 0);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(51, 45);
            button4.TabIndex = 7;
            button4.Text = "X";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14F);
            label3.ForeColor = Color.White;
            label3.Location = new Point(95, 94);
            label3.Name = "label3";
            label3.Size = new Size(97, 25);
            label3.TabIndex = 6;
            label3.Text = "Username";
            label3.Visible = false;
            label3.Click += label3_Click;
            // 
            // guna2TextBox1
            // 
            guna2TextBox1.Animated = true;
            guna2TextBox1.BorderRadius = 14;
            guna2TextBox1.CustomizableEdges = customizableEdges1;
            guna2TextBox1.DefaultText = "";
            guna2TextBox1.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            guna2TextBox1.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            guna2TextBox1.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            guna2TextBox1.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            guna2TextBox1.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            guna2TextBox1.Font = new Font("Segoe UI", 9F);
            guna2TextBox1.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            guna2TextBox1.Location = new Point(95, 122);
            guna2TextBox1.Margin = new Padding(3, 4, 3, 4);
            guna2TextBox1.Name = "guna2TextBox1";
            guna2TextBox1.PlaceholderText = "";
            guna2TextBox1.SelectedText = "";
            guna2TextBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2TextBox1.Size = new Size(345, 31);
            guna2TextBox1.TabIndex = 5;
            guna2TextBox1.Visible = false;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 7F);
            label2.ForeColor = Color.White;
            label2.Location = new Point(3, 16);
            label2.Name = "label2";
            label2.Size = new Size(28, 12);
            label2.TabIndex = 4;
            label2.Text = "Value";
            label2.Visible = false;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.BackColor = Color.FromArgb(60, 63, 68);
            button3.Cursor = Cursors.Hand;
            button3.FlatAppearance.BorderColor = Color.FromArgb(90, 93, 98);
            button3.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 53, 58);
            button3.FlatAppearance.MouseOverBackColor = Color.FromArgb(75, 78, 84);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI Semibold", 10F);
            button3.ForeColor = Color.White;
            button3.Location = new Point(318, 240);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(232, 40);
            button3.TabIndex = 3;
            button3.Text = "Çevrimdışı Oyna";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F);
            label1.ForeColor = Color.White;
            label1.Location = new Point(163, 6);
            label1.Name = "label1";
            label1.Size = new Size(206, 25);
            label1.TabIndex = 2;
            label1.Text = "Select an Account Type";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.BackColor = Color.FromArgb(0, 120, 215);
            button2.Cursor = Cursors.Hand;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 99, 177);
            button2.FlatAppearance.MouseOverBackColor = Color.FromArgb(16, 137, 231);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Semibold", 10F);
            button2.ForeColor = Color.White;
            button2.Location = new Point(8, 240);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(225, 40);
            button2.TabIndex = 1;
            button2.Text = "Microsoft ile Giriş Yap";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom;
            button1.BackColor = Color.FromArgb(52, 152, 219);
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            button1.ForeColor = Color.White;
            button1.Location = new Point(293, 467);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(293, 40);
            button1.TabIndex = 0;
            button1.Text = "Add Account";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_1;
            // 
            // AccountManager
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(20, 20, 20);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(flowAccounts);
            Margin = new Padding(4, 3, 4, 3);
            Name = "AccountManager";
            Size = new Size(884, 521);
            Load += AccountManager_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        private Button button1;
        private Panel panel1;
        private Button button3;
        private Label label1;
        private Button button2;
        private Label label2;
        private Label label3;
        private Guna.UI2.WinForms.Guna2TextBox guna2TextBox1;
        private Button button4;
    }
}

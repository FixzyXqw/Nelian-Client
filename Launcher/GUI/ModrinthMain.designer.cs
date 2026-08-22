using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
namespace Nelian
{
    partial class ModrinthMain
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                fxTimer?.Stop();
                fxTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Designer generated fields
        private Panel headerPanel;
        private Panel headerAccentLine;
        private Label headerIconLabel;
        private Label titleLabel;
        private Label subtitleLabel;
        private Guna2Button addButton;
        private Guna2Button backButton;
        private ModernFlowLayoutPanel instancePanel;
        private Panel progressPanel;
        private Panel progressAccentLine;
        private Guna2ProgressBar progressBar;
        private Label progressStatusLabel;
        private Label progressDetailLabel;
        private Label progressPercentLabel;
        #endregion
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            headerPanel = new Panel();
            headerAccentLine = new Panel();
            headerIconLabel = new Label();
            titleLabel = new Label();
            subtitleLabel = new Label();
            backButton = new Guna2Button();
            addButton = new Guna2Button();
            instancePanel = new ModernFlowLayoutPanel();
            progressPanel = new Panel();
            progressAccentLine = new Panel();
            progressStatusLabel = new Label();
            progressPercentLabel = new Label();
            progressBar = new Guna2ProgressBar();
            progressDetailLabel = new Label();
            headerPanel.SuspendLayout();
            progressPanel.SuspendLayout();
            SuspendLayout();
            headerPanel.BackColor = Color.FromArgb(12, 15, 22);
            headerPanel.Controls.Add(headerAccentLine);
            headerPanel.Controls.Add(headerIconLabel);
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            headerPanel.Controls.Add(backButton);
            headerPanel.Controls.Add(addButton);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(884, 64);
            headerPanel.TabIndex = 2;
            headerAccentLine.BackColor = Color.Transparent;
            headerAccentLine.Dock = DockStyle.Bottom;
            headerAccentLine.Location = new Point(0, 62);
            headerAccentLine.Name = "headerAccentLine";
            headerAccentLine.Size = new Size(884, 2);
            headerAccentLine.TabIndex = 0;
            headerIconLabel.AutoSize = true;
            headerIconLabel.BackColor = Color.Transparent;
            headerIconLabel.Font = new Font("Segoe Fluent Icons", 20F);
            headerIconLabel.ForeColor = Color.FromArgb(0, 210, 255);
            headerIconLabel.Location = new Point(18, 16);
            headerIconLabel.Name = "headerIconLabel";
            headerIconLabel.Size = new Size(39, 27);
            headerIconLabel.TabIndex = 1;
            headerIconLabel.Text = "";
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(235, 242, 255);
            titleLabel.Location = new Point(52, 8);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(92, 25);
            titleLabel.TabIndex = 2;
            titleLabel.Text = "Instances";
            subtitleLabel.AutoSize = true;
            subtitleLabel.BackColor = Color.Transparent;
            subtitleLabel.Font = new Font("Segoe UI", 8.5F);
            subtitleLabel.ForeColor = Color.FromArgb(140, 155, 180);
            subtitleLabel.Location = new Point(53, 33);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(0, 15);
            subtitleLabel.TabIndex = 3;
            backButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            backButton.BorderRadius = 12;
            backButton.Cursor = Cursors.Hand;
            backButton.CustomizableEdges = customizableEdges1;
            backButton.FillColor = Color.FromArgb(36, 255, 255, 255);
            backButton.Font = new Font("Segoe Fluent Icons", 15F);
            backButton.ForeColor = Color.FromArgb(235, 242, 255);
            backButton.HoverState.FillColor = Color.FromArgb(60, 255, 90, 90);
            backButton.Location = new Point(884, 13);
            backButton.Name = "backButton";
            backButton.ShadowDecoration.CustomizableEdges = customizableEdges2;
            backButton.Size = new Size(38, 38);
            backButton.TabIndex = 4;
            backButton.Text = "";
            backButton.TextOffset = new Point(0, -1);
            addButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            addButton.BorderRadius = 12;
            addButton.Cursor = Cursors.Hand;
            addButton.CustomizableEdges = customizableEdges3;
            addButton.FillColor = Color.FromArgb(0, 210, 255);
            addButton.Font = new Font("Segoe Fluent Icons", 15F);
            addButton.ForeColor = Color.FromArgb(8, 12, 18);
            addButton.HoverState.FillColor = Color.FromArgb(60, 225, 255);
            addButton.Location = new Point(884, 13);
            addButton.Name = "addButton";
            addButton.ShadowDecoration.CustomizableEdges = customizableEdges4;
            addButton.Size = new Size(38, 38);
            addButton.TabIndex = 5;
            addButton.Text = "";
            addButton.TextOffset = new Point(0, -1);
            instancePanel.AutoScroll = true;
            instancePanel.BackColor = Color.FromArgb(15, 18, 26);
            instancePanel.Dock = DockStyle.Fill;
            instancePanel.FlowDirection = FlowDirection.TopDown;
            instancePanel.Location = new Point(0, 64);
            instancePanel.Name = "instancePanel";
            instancePanel.Padding = new Padding(14);
            instancePanel.Size = new Size(884, 381);
            instancePanel.TabIndex = 0;
            instancePanel.WrapContents = false;
            instancePanel.Paint += instancePanel_Paint;
            progressPanel.BackColor = Color.FromArgb(12, 15, 22);
            progressPanel.Controls.Add(progressAccentLine);
            progressPanel.Controls.Add(progressStatusLabel);
            progressPanel.Controls.Add(progressPercentLabel);
            progressPanel.Controls.Add(progressBar);
            progressPanel.Controls.Add(progressDetailLabel);
            progressPanel.Dock = DockStyle.Bottom;
            progressPanel.Location = new Point(0, 445);
            progressPanel.Name = "progressPanel";
            progressPanel.Size = new Size(884, 76);
            progressPanel.TabIndex = 1;
            progressPanel.Visible = false;
            progressAccentLine.BackColor = Color.Transparent;
            progressAccentLine.Dock = DockStyle.Top;
            progressAccentLine.Location = new Point(0, 0);
            progressAccentLine.Name = "progressAccentLine";
            progressAccentLine.Size = new Size(884, 2);
            progressAccentLine.TabIndex = 0;
            progressStatusLabel.AutoSize = true;
            progressStatusLabel.BackColor = Color.Transparent;
            progressStatusLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            progressStatusLabel.ForeColor = Color.FromArgb(235, 242, 255);
            progressStatusLabel.Location = new Point(18, 16);
            progressStatusLabel.Name = "progressStatusLabel";
            progressStatusLabel.Size = new Size(115, 17);
            progressStatusLabel.TabIndex = 1;
            progressStatusLabel.Text = "Starting process...";
            progressPercentLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            progressPercentLabel.AutoSize = true;
            progressPercentLabel.BackColor = Color.Transparent;
            progressPercentLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            progressPercentLabel.ForeColor = Color.FromArgb(0, 210, 255);
            progressPercentLabel.Location = new Point(884, 16);
            progressPercentLabel.Name = "progressPercentLabel";
            progressPercentLabel.Size = new Size(26, 17);
            progressPercentLabel.TabIndex = 2;
            progressPercentLabel.Text = "0%";
            progressBar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.BorderRadius = 5;
            progressBar.CustomizableEdges = customizableEdges5;
            progressBar.FillColor = Color.FromArgb(40, 48, 66);
            progressBar.Location = new Point(18, 42);
            progressBar.Name = "progressBar";
            progressBar.ProgressColor = Color.FromArgb(0, 210, 255);
            progressBar.ShadowDecoration.CustomizableEdges = customizableEdges6;
            progressBar.Size = new Size(884, 10);
            progressBar.TabIndex = 3;
            progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            progressDetailLabel.AutoSize = true;
            progressDetailLabel.BackColor = Color.Transparent;
            progressDetailLabel.Font = new Font("Segoe UI", 8F);
            progressDetailLabel.ForeColor = Color.FromArgb(140, 155, 180);
            progressDetailLabel.Location = new Point(18, 58);
            progressDetailLabel.Name = "progressDetailLabel";
            progressDetailLabel.Size = new Size(0, 13);
            progressDetailLabel.TabIndex = 4;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 18, 26);
            Controls.Add(instancePanel);
            Controls.Add(progressPanel);
            Controls.Add(headerPanel);
            Name = "ModrinthMain";
            Size = new Size(884, 521);
            Load += ModrinthMain_Load;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            progressPanel.ResumeLayout(false);
            progressPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
    internal static class ModrinthColors
    {
        public static readonly Color BgColor = Color.FromArgb(15, 18, 26);
        public static readonly Color SidebarColor = Color.FromArgb(12, 15, 22);
        public static readonly Color CardBgColor = Color.FromArgb(24, 29, 41);
        public static readonly Color CardHoverBgColor = Color.FromArgb(31, 38, 54);
        public static readonly Color AccentA = Color.FromArgb(0, 210, 255);
        public static readonly Color AccentB = Color.FromArgb(140, 90, 255);
        public static readonly Color CardBorderColor = Color.FromArgb(28, 255, 255, 255);
        public static readonly Color CardHoverBorderColor = Color.FromArgb(120, 0, 210, 255);
        public static readonly Color TextPrimary = Color.FromArgb(235, 242, 255);
        public static readonly Color TextSecondary = Color.FromArgb(140, 155, 180);
    }
    internal static class ModrinthStyle
    {
        private static Font _headerIconFont;
        private static Font _buttonIconFont;
        private static Font _cardIconFont;
        private static readonly object _lock = new object();
        public static Font HeaderIconFont { get { EnsureFonts(); return _headerIconFont; } }
        public static Font ButtonIconFont { get { EnsureFonts(); return _buttonIconFont; } }
        public static Font CardIconFont { get { EnsureFonts(); return _cardIconFont; } }
        private static void EnsureFonts()
        {
            lock (_lock)
            {
                if (_headerIconFont == null) _headerIconFont = GetIconFont(20);
                if (_buttonIconFont == null) _buttonIconFont = GetIconFont(15);
                if (_cardIconFont == null) _cardIconFont = GetIconFont(19);
            }
        }
        private static Font GetIconFont(float size)
        {
            try
            {
                using (var testFont = new Font("Segoe Fluent Icons", size))
                {
                    if (testFont.Name == "Segoe Fluent Icons")
                        return new Font("Segoe Fluent Icons", size, FontStyle.Regular);
                }
            }
            catch { }
            return new Font("Segoe MDL2 Assets", size, FontStyle.Regular);
        }
    }
}

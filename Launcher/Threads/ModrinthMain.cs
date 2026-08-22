using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using Guna.UI2.WinForms;
using Nelian.Installer;
using Nelian.Managers;
using Nelian.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Nelian
{
    public partial class ModrinthMain : UserControl
    {
        public event EventHandler<Instance> InstanceLaunchRequested;
        public event EventHandler<string> LaunchStatusChanged;
        public event EventHandler BackRequested;
        private readonly Dictionary<Panel, float> cardHoverProgress = new Dictionary<Panel, float>();
        private readonly Dictionary<Panel, bool> cardHoverTarget = new Dictionary<Panel, bool>();
        private System.Windows.Forms.Timer fxTimer;
        private readonly ToolTip sharedToolTip = new ToolTip();
        public ModrinthMain()
        {
            InitializeComponent();
            ApplyVisualEffects();
            InitializeContentFeature();
            addButton.Location = new Point(headerPanel.Width - addButton.Width - 16, 13);
            backButton.Location = new Point(addButton.Location.X - backButton.Width - 8, 13);
            WireEvents();
            LoadInstances();
        }
        private void ApplyVisualEffects()
        {
            EnableDoubleBuffer(headerPanel);
            EnableDoubleBuffer(progressPanel);
            EnableDoubleBuffer(instancePanel);
            headerAccentLine.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(headerAccentLine.Width, 0),
                    Color.FromArgb(10, ModrinthColors.AccentA.R, ModrinthColors.AccentA.G, ModrinthColors.AccentA.B),
                    Color.FromArgb(160, ModrinthColors.AccentB.R, ModrinthColors.AccentB.G, ModrinthColors.AccentB.B)))
                {
                    e.Graphics.FillRectangle(brush, headerAccentLine.ClientRectangle);
                }
            };
            progressAccentLine.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(progressAccentLine.Width, 0),
                    Color.FromArgb(160, ModrinthColors.AccentA.R, ModrinthColors.AccentA.G, ModrinthColors.AccentA.B),
                    Color.FromArgb(10, ModrinthColors.AccentB.R, ModrinthColors.AccentB.G, ModrinthColors.AccentB.B)))
                {
                    e.Graphics.FillRectangle(brush, progressAccentLine.ClientRectangle);
                }
            };
            fxTimer = new System.Windows.Forms.Timer { Interval = 15 };
            fxTimer.Tick += FxTimer_Tick;
            fxTimer.Start();
        }
        private void FxTimer_Tick(object sender, EventArgs e)
        {
            foreach (var card in cardHoverProgress.Keys.ToList())
            {
                if (card.IsDisposed)
                {
                    cardHoverProgress.Remove(card);
                    cardHoverTarget.Remove(card);
                    continue;
                }
                float current = cardHoverProgress[card];
                bool target = cardHoverTarget.TryGetValue(card, out var tgt) && tgt;
                float goal = target ? 1f : 0f;
                if (Math.Abs(current - goal) > 0.003f)
                {
                    current += (goal - current) * 0.22f;
                    cardHoverProgress[card] = current;
                    card.Invalidate();
                }
            }
        }
        private void EnableDoubleBuffer(Control c)
        {
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(c, true);
        }
        private static Color Lerp(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                a.A + (int)((b.A - a.A) * t),
                a.R + (int)((b.R - a.R) * t),
                a.G + (int)((b.G - a.G) * t),
                a.B + (int)((b.B - a.B) * t));
        }
        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
        private void WireEvents()
        {
            addButton.Click += AddButton_Click;
            backButton.Click += (s, e) => BackRequested?.Invoke(this, EventArgs.Empty);
            addButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            backButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Resize += (s, e) =>
            {
                addButton.Location = new Point(headerPanel.Width - addButton.Width - 16, 13);
                backButton.Location = new Point(addButton.Location.X - backButton.Width - 8, 13);
                foreach (Control ctrl in instancePanel.Controls)
                {
                    if (!(ctrl is Panel card)) continue;
                    card.Width = instancePanel.ClientSize.Width - instancePanel.Padding.Horizontal - 4;
                    if (cardContentStates.TryGetValue(card, out var state) && state.ContentHost != null)
                    {
                        state.ContentHost.Width = card.Width - 24;
                        foreach (var tabPanel in state.TabListPanels.Values)
                        {
                            if (tabPanel != null)
                                tabPanel.Width = state.ContentHost.Width;
                        }
                        RelayoutContentRows(state);
                    }
                }
            };
        }
        public void ShowProgress(string status, int value = 0, int max = 100, string detail = "")
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => ShowProgress(status, value, max, detail)));
                return;
            }
            progressPanel.Visible = true;
            progressStatusLabel.Text = status;
            progressBar.Maximum = max;
            progressBar.Value = Math.Min(value, max);
            progressPercentLabel.Text = $"{Math.Min(value, max)}%";
            progressDetailLabel.Text = detail;
            LaunchStatusChanged?.Invoke(this, status);
        }
        public void HideProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => HideProgress()));
                return;
            }
            progressPanel.Visible = false;
            progressBar.Value = 0;
            progressPercentLabel.Text = "0%";
            progressDetailLabel.Text = "";
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = LanguageManager.Get("Modrinth.PackFilter");
                openFileDialog.Title = LanguageManager.Get("Modrinth.SelectPack");
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    _ = InstallPackAsync(filePath);
                }
            }
        }
        private async Task InstallPackAsync(string filePath)
        {
            try
            {
                this.Enabled = false;
                ShowProgress(LanguageManager.Get("Modrinth.ReadingPack"), 0, 100, LanguageManager.Get("Modrinth.ScanningFile"));
                var success = await MrPackInstaller.InstallAsync(filePath,
                    onProgress: (status, current, total) =>
                    {
                        int percent = total > 0 ? (int)((double)current / total * 100) : 0;
                        ShowProgress(status, percent, 100, status);
                    }
                );
                if (success)
                {
                    ShowProgress(LanguageManager.Get("Modrinth.Complete"), 100, 100, LanguageManager.Get("Modrinth.InstanceCreated"));
                    await Task.Delay(500);
                    LoadInstances();
                }
            }
            catch (Exception ex)
            {
                ShowProgress(LanguageManager.Get("Modrinth.Error"), 0, 100, ex.Message);
                MessageBox.Show(string.Format(LanguageManager.Get("Modrinth.ErrorMessage"), ex.Message), LanguageManager.Get("Modrinth.ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                await Task.Delay(1000);
                HideProgress();
                this.Enabled = true;
            }
        }
        public void LoadInstances()
        {
            foreach (Control ctrl in instancePanel.Controls)
            {
                if (ctrl is Panel card)
                    DisposeCardResources(card);
            }
            instancePanel.Controls.Clear();
            cardHoverProgress.Clear();
            cardHoverTarget.Clear();
            cardExpandProgress.Clear();
            cardExpandTarget.Clear();
            cardContentStates.Clear();
            var instances = InstanceManager.GetAll();
            if (instances.Count == 0)
            {
                var emptyPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
                var iconLbl = new Label
                {
                    Text = "\uE7C1",
                    Font = ModrinthStyle.HeaderIconFont,
                    ForeColor = Color.FromArgb(60, 70, 90),
                    AutoSize = false,
                    Height = 60,
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.BottomCenter,
                    BackColor = Color.Transparent
                };
                var lbl = new Label
                {
                    Text = LanguageManager.Get("Modrinth.NoInstances"),
                    ForeColor = ModrinthColors.TextSecondary,
                    Font = new Font("Segoe UI", 12),
                    TextAlign = ContentAlignment.TopCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };
                emptyPanel.Controls.Add(lbl);
                emptyPanel.Controls.Add(iconLbl);
                instancePanel.Controls.Add(emptyPanel);
                return;
            }
            instances.Sort((a, b) => string.Compare(a.Name, b.Name));
            instancePanel.SuspendLayout();
            foreach (var instance in instances)
                instancePanel.Controls.Add(CreateInstanceCard(instance));
            instancePanel.ResumeLayout();
        }
        private void DisposeCardResources(Panel card)
        {
            void Walk(Control c)
            {
                foreach (Control child in c.Controls)
                {
                    Walk(child);
                    if (child is PictureBox pb && pb.Image != null)
                    {
                        var img = pb.Image;
                        pb.Image = null;
                        img.Dispose();
                    }
                }
            }
            try { Walk(card); } catch { }
            card.Dispose();
        }
        private (string icon, Color color, string label) GetLoaderVisual(string loader)
        {
            switch (loader?.ToLower())
            {
                case "fabric": return ("\uE945", Color.FromArgb(220, 130, 60), LanguageManager.Get("Modrinth.Fabric"));
                case "forge": return ("\uE90F", Color.FromArgb(140, 120, 255), LanguageManager.Get("Modrinth.Forge"));
                case "neoforge": return ("\uE90F", Color.FromArgb(255, 140, 60), "NeoForge");
                case "quilt": return ("\uE99A", Color.FromArgb(150, 90, 220), LanguageManager.Get("Modrinth.Quilt"));
                default: return ("\uE74C", ModrinthColors.AccentA, "Vanilla");
            }
        }
        private Panel CreateInstanceCard(Instance instance)
        {
            int cardWidth = Math.Max(200, instancePanel.ClientSize.Width - instancePanel.Padding.Horizontal - 4);
            var card = new Panel
            {
                Width = cardWidth,
                Height = 92,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 10),
                Tag = instance
            };
            EnableDoubleBuffer(card);
            cardHoverProgress[card] = 0f;
            cardHoverTarget[card] = false;
            card.Paint += (s, e) =>
            {
                float hover = cardHoverProgress.TryGetValue(card, out var hp) ? hp : 0f;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var path = GetRoundedPath(rect, 14))
                {
                    Color bg = Lerp(ModrinthColors.CardBgColor, ModrinthColors.CardHoverBgColor, hover);
                    using (var brush = new SolidBrush(bg))
                        e.Graphics.FillPath(brush, path);
                    Color reflect = Color.FromArgb((int)(8 + hover * 14), 255, 255, 255);
                    using (var gb = new LinearGradientBrush(new Point(0, 0), new Point(0, card.Height / 2), reflect, Color.Transparent))
                        e.Graphics.FillPath(gb, path);
                    Color border = Lerp(ModrinthColors.CardBorderColor, ModrinthColors.CardHoverBorderColor, hover);
                    using (var pen = new Pen(border, 1f + hover * 0.4f))
                        e.Graphics.DrawPath(pen, path);
                }
                if (hover > 0.01f)
                {
                    using (var accentPen = new Pen(Color.FromArgb((int)(200 * hover), ModrinthColors.AccentA.R, ModrinthColors.AccentA.G, ModrinthColors.AccentA.B), 3))
                        e.Graphics.DrawLine(accentPen, 1, 12, 1, card.Height - 12);
                }
            };
            void OnEnter(object s, EventArgs e) => cardHoverTarget[card] = true;
            void OnLeave(object s, EventArgs e)
            {
                if (!card.IsDisposed && !card.ClientRectangle.Contains(card.PointToClient(Cursor.Position)))
                    cardHoverTarget[card] = false;
            }
            card.MouseEnter += OnEnter;
            card.MouseLeave += OnLeave;
            card.ControlAdded += (s, e) =>
            {
                e.Control.MouseEnter += OnEnter;
                e.Control.MouseLeave += OnLeave;
            };
            var (loaderIcon, loaderColor, loaderLabel) = GetLoaderVisual(instance.Loader);
            var badge = new Panel { Size = new Size(46, 46), Location = new Point(16, 16), BackColor = Color.Transparent };
            badge.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var r = new Rectangle(0, 0, badge.Width - 1, badge.Height - 1);
                using (var path = GetRoundedPath(r, 12))
                using (var brush = new SolidBrush(Color.FromArgb(36, loaderColor.R, loaderColor.G, loaderColor.B)))
                    e.Graphics.FillPath(brush, path);
            };
            var badgeIcon = new Label
            {
                Text = loaderIcon,
                Font = ModrinthStyle.CardIconFont,
                ForeColor = loaderColor,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            badge.Controls.Add(badgeIcon);
            card.Controls.Add(badge);
            var nameLabel = new Label
            {
                Text = instance.Name,
                ForeColor = ModrinthColors.TextPrimary,
                Font = new Font("Segoe UI Semibold", 12, FontStyle.Bold),
                Location = new Point(74, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(nameLabel);
            var versionLabel = new Label
            {
                Text = string.Format(LanguageManager.Get("Modrinth.VersionFormat"), instance.MinecraftVersion, loaderLabel, instance.LoaderVersion),
                ForeColor = ModrinthColors.TextSecondary,
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(75, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(versionLabel);
            var launchStatusLabel = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(255, 200, 0),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                Location = new Point(75, 60),
                AutoSize = true,
                Visible = false,
                BackColor = Color.Transparent,
                Name = "launchStatus"
            };
            card.Controls.Add(launchStatusLabel);
            var contentBtn = new Guna2Button
            {
                Text = "\uE70D",
                Font = ModrinthStyle.ButtonIconFont,
                Size = new Size(38, 34),
                Location = new Point(cardWidth - 176, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.FromArgb(45, 52, 70),
                ForeColor = ModrinthColors.AccentA,
                BorderRadius = 9,
                Cursor = Cursors.Hand
            };
            contentBtn.HoverState.FillColor = Color.FromArgb(58, 66, 88);
            sharedToolTip.SetToolTip(contentBtn, LanguageManager.Get("Modrinth.Content") ?? "İçerik");
            contentBtn.Click += async (s, e) => await ToggleCardContentAsync(card, instance, contentBtn);
            card.Controls.Add(contentBtn);
            cardExpandProgress[card] = 0f;
            cardExpandTarget[card] = false;
            var playBtn = new Guna2Button
            {
                Text = "\uE768",
                Font = ModrinthStyle.ButtonIconFont,
                Size = new Size(38, 34),
                Location = new Point(cardWidth - 132, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.FromArgb(0, 200, 100),
                ForeColor = Color.White,
                BorderRadius = 9,
                TextOffset = new Point(1, -1),
                Cursor = Cursors.Hand,
                Tag = instance
            };
            playBtn.HoverState.FillColor = Color.FromArgb(0, 180, 80);
            sharedToolTip.SetToolTip(playBtn, LanguageManager.Get("Modrinth.Play"));
            playBtn.Click += PlayButton_Click;
            card.Controls.Add(playBtn);
            var folderBtn = new Guna2Button
            {
                Text = "\uE838",
                Font = ModrinthStyle.ButtonIconFont,
                Size = new Size(38, 34),
                Location = new Point(cardWidth - 88, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.FromArgb(45, 52, 70),
                ForeColor = ModrinthColors.TextPrimary,
                BorderRadius = 9,
                Cursor = Cursors.Hand
            };
            folderBtn.HoverState.FillColor = Color.FromArgb(58, 66, 88);
            sharedToolTip.SetToolTip(folderBtn, LanguageManager.Get("Modrinth.OpenFolder") ?? "Klasörü Aç");
            folderBtn.Click += (s, e) =>
            {
                if (Directory.Exists(instance.Path))
                    Process.Start("explorer.exe", instance.Path);
            };
            card.Controls.Add(folderBtn);
            var deleteBtn = new Guna2Button
            {
                Text = "\uE74D",
                Font = ModrinthStyle.ButtonIconFont,
                Size = new Size(38, 34),
                Location = new Point(cardWidth - 44, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FillColor = Color.FromArgb(60, 200, 60, 60),
                ForeColor = Color.FromArgb(255, 140, 140),
                BorderRadius = 9,
                Cursor = Cursors.Hand
            };
            deleteBtn.HoverState.FillColor = Color.FromArgb(200, 50, 50);
            sharedToolTip.SetToolTip(deleteBtn, LanguageManager.Get("Modrinth.Delete") ?? "Sil");
            deleteBtn.Click += (s, e) => DeleteInstance(instance);
            card.Controls.Add(deleteBtn);
            return card;
        }
        private async void PlayButton_Click(object sender, EventArgs e)
        {
            var button = sender as Guna2Button;
            var instance = button?.Tag as Instance;
            if (instance == null) return;
            if (Main._session == null)
            {
                MessageBox.Show(LanguageManager.Get("Modrinth.NoSessionFound"), LanguageManager.Get("Modrinth.ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var panel = button.Parent as Panel;
            var statusLabel = panel?.Controls["launchStatus"] as Label;
            try
            {
                button.Enabled = false;
                button.Text = "\uE9F5";
                button.FillColor = Color.FromArgb(255, 200, 0);
                if (statusLabel != null)
                {
                    statusLabel.Visible = true;
                    statusLabel.Text = LanguageManager.Get("Modrinth.Preparing");
                    statusLabel.ForeColor = Color.FromArgb(255, 200, 0);
                }
                ShowProgress(string.Format(LanguageManager.Get("Modrinth.PreparingInstance"), instance.Name), 10, 100, LanguageManager.Get("Modrinth.ScanningFiles"));
                var minecraftPath = new MinecraftPath(instance.Path);
                var launcher = new MinecraftLauncher(minecraftPath);
                string versionToLaunch;
                if (!string.IsNullOrWhiteSpace(instance.VersionId))
                {
                    versionToLaunch = instance.VersionId;
                    ShowProgress(string.Format(LanguageManager.Get("Modrinth.ReadyVersion"), versionToLaunch), 30, 100, LanguageManager.Get("Modrinth.UsingExisting"));
                }
                else
                {
                    switch (instance.Loader?.ToLower())
                    {
                        case "forge":
                            ShowProgress(LanguageManager.Get("Modrinth.InstallingForge"), 25, 100, string.Format(LanguageManager.Get("Modrinth.ForgeVersion"), instance.LoaderVersion));
                            versionToLaunch = await ForgeInstaller.InstallAsync(
                                instance.Path,
                                instance.MinecraftVersion,
                                instance.LoaderVersion);
                            ShowProgress(string.Format(LanguageManager.Get("Modrinth.ForgeInstalled"), instance.LoaderVersion), 40, 100, LanguageManager.Get("Modrinth.ForgeReady"));
                            break;
                        case "neoforge":
                            ShowProgress(LanguageManager.Get("Modrinth.InstallingNeoForge"), 25, 100, string.Format(LanguageManager.Get("Modrinth.NeoForgeVersion"), instance.LoaderVersion));
                            versionToLaunch = await NeoForgeInstaller.InstallAsync(
                                instance.Path,
                                instance.MinecraftVersion,
                                instance.LoaderVersion);
                            ShowProgress(string.Format(LanguageManager.Get("Modrinth.NeoForgeInstalled"), instance.LoaderVersion), 40, 100, LanguageManager.Get("Modrinth.NeoForgeReady"));
                            break;
                        case "fabric":
                            ShowProgress(LanguageManager.Get("Modrinth.InstallingFabric"), 25, 100, string.Format(LanguageManager.Get("Modrinth.FabricVersion"), instance.LoaderVersion));
                            string fabricMetaUrl = $"https:
                            using (var client = new WebClient())
                            {
                                string json = await client.DownloadStringTaskAsync(fabricMetaUrl);
                                var doc = System.Text.Json.JsonDocument.Parse(json);
                                versionToLaunch = doc.RootElement.GetProperty("id").GetString();
                                string versionDir = Path.Combine(instance.Path, "versions", versionToLaunch);
                                Directory.CreateDirectory(versionDir);
                                string versionJsonPath = Path.Combine(versionDir, $"{versionToLaunch}.json");
                                File.WriteAllText(versionJsonPath, json);
                            }
                            ShowProgress(string.Format(LanguageManager.Get("Modrinth.FabricInstalled"), instance.LoaderVersion), 40, 100, LanguageManager.Get("Modrinth.FabricReady"));
                            break;
                        default:
                            versionToLaunch = instance.MinecraftVersion;
                            ShowProgress(string.Format(LanguageManager.Get("Modrinth.UsingVanilla"), versionToLaunch), 30, 100, LanguageManager.Get("Modrinth.VanillaVersion"));
                            break;
                    }
                    instance.VersionId = versionToLaunch;
                    try
                    {
                        InstanceManager.UpdateInstance(instance);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Instance güncellenirken hata: {ex.Message}");
                    }
                }
                var launchOption = new MLaunchOption
                {
                    Session = Main._session,
                    MaximumRamMb = int.Parse(SettingsHelper.GetMemory().ToString())
                };
                InstanceLaunchRequested?.Invoke(this, instance);
                ShowProgress(string.Format(LanguageManager.Get("Modrinth.StartingInstance"), instance.Name), 50, 100, string.Format(LanguageManager.Get("Modrinth.StartingVersion"), versionToLaunch));
                var process = await launcher.CreateProcessAsync(versionToLaunch, launchOption);
                string lastError = LanguageManager.Get("Modrinth.UnknownError");
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.OutputDataReceived += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                    {
                        Console.WriteLine($"[{instance.Name}] {ev.Data}");
                        if (ev.Data.Contains("Exception") || ev.Data.Contains("Error") || ev.Data.Contains("Fatal"))
                            lastError = ev.Data;
                    }
                };
                process.ErrorDataReceived += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(ev.Data))
                    {
                        Console.WriteLine($"[{instance.Name}-ERR] {ev.Data}");
                        lastError = ev.Data;
                    }
                };
                process.EnableRaisingEvents = true;
                process.Exited += (s, ev) =>
                {
                    try
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            if (process.ExitCode != 0)
                            {
                                if (statusLabel != null)
                                {
                                    statusLabel.ForeColor = Color.Red;
                                    statusLabel.Text = LanguageManager.Get("Modrinth.Crashed");
                                }
                                MessageBox.Show(
                                    string.Format(LanguageManager.Get("Modrinth.CrashMessage"), lastError, process.ExitCode),
                                    LanguageManager.Get("Modrinth.CrashDetected"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (statusLabel != null)
                                {
                                    statusLabel.ForeColor = Color.FromArgb(0, 200, 100);
                                    statusLabel.Text = LanguageManager.Get("Modrinth.Closed");
                                }
                            }
                        }));
                    }
                    catch { }
                };
                process.Start();
                System.Diagnostics.Debug.WriteLine(process.StartInfo.FileName);
                System.Diagnostics.Debug.WriteLine(process.StartInfo.Arguments);
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                ShowProgress(string.Format(LanguageManager.Get("Modrinth.InstanceRunning"), instance.Name), 100, 100, LanguageManager.Get("Modrinth.WaitingForWindow"));
                await Task.Delay(1500);
                HideProgress();
                if (statusLabel != null)
                {
                    statusLabel.Text = LanguageManager.Get("Modrinth.Running");
                    statusLabel.ForeColor = Color.FromArgb(0, 200, 100);
                }
                instance.LastPlayed = DateTime.Now;
                try
                {
                    InstanceManager.UpdateInstance(instance);
                }
                catch { }
                while (!process.HasExited)
                {
                    process.Refresh();
                    if (process.MainWindowHandle != IntPtr.Zero)
                        break;
                    await Task.Delay(100);
                }
                await Task.Run(() => process.WaitForExit());
                button.Enabled = true;
                button.Text = "\uE768";
                button.FillColor = Color.FromArgb(0, 200, 100);
                if (statusLabel != null)
                {
                    statusLabel.Visible = false;
                    statusLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = LanguageManager.Get("Modrinth.Error");
                    statusLabel.ForeColor = Color.FromArgb(200, 50, 50);
                }
                HideProgress();
                MessageBox.Show(
                    string.Format(LanguageManager.Get("Modrinth.LaunchError"), ex.Message),
                    LanguageManager.Get("Modrinth.ErrorTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                button.Enabled = true;
                button.Text = "\uE768";
                button.FillColor = Color.FromArgb(0, 200, 100);
                if (statusLabel != null)
                {
                    statusLabel.Visible = false;
                    statusLabel.Text = "";
                }
            }
        }
        private void DeleteInstance(Instance instance)
        {
            var result = MessageBox.Show(
                string.Format(LanguageManager.Get("Modrinth.DeleteConfirm"), instance.Name),
                LanguageManager.Get("Modrinth.DeleteTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );
            if (result == DialogResult.Yes)
            {
                InstanceManager.Delete(instance.Name);
                LoadInstances();
            }
        }
        private void ModrinthMain_Load(object sender, EventArgs e)
        {
        }
        private void instancePanel_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}

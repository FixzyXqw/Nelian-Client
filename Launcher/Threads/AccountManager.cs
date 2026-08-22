using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.Sessions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace Nelian
{
    public partial class AccountManager : UserControl
    {
        private readonly JELoginHandler _loginHandler;
        public event Action<MSession>? OpenMainRequested;
        private bool _isAuthenticating = false;
        private static bool _autoLoginDone = false;
        private Panel _autoLoginPanel;
        private Label _autoLoginLabel;
        private Label _autoLoginaltLabel;
        private List<JEGameAccount> _cachedAccounts = new();
        private System.Windows.Forms.Timer _hidePanelTimer;
        private bool isOfflineMode = false;
        public AccountManager(JELoginHandler loginHandler)
        {
            InitializeComponent();
            _loginHandler = loginHandler;
            AutoScaleMode = AutoScaleMode.None;
            if (button1 != null)
                button1.Text = LanguageManager.Get("AccountManager.AddAccount");
            if (button2 != null)
                button2.Text = LanguageManager.Get("AccountManager.Microsoft");
            if (button3 != null)
                button3.Text = LanguageManager.Get("AccountManager.OfflineMode");
            if (label1 != null)
                label1.Text = LanguageManager.Get("AccountManager.SelectAccountType");
            if (label3 != null)
                label3.Text = LanguageManager.Get("AccountManager.Username");
            this.Load += AccountManager_Load;
            if (button1 != null)
            {
                button1.Click += button1_Click_1;
            }
            if (flowAccounts != null)
            {
                flowAccounts.FlowDirection = FlowDirection.LeftToRight;
                flowAccounts.WrapContents = true;
                flowAccounts.AutoScroll = true;
                flowAccounts.Padding = new Padding(10);
            }
            CreateAutoLoginPanel();
            _hidePanelTimer = new System.Windows.Forms.Timer();
            _hidePanelTimer.Interval = 2000;
            _hidePanelTimer.Tick += HidePanelTimer_Tick;
            Console.WriteLine("AccountManager constructor called");
        }
        private string GetOfflineKeyFilePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft",
                "Game",
                "Nelian",
                "Offline.key"
            );
        }
        private List<string> ReadOfflineUsernames()
        {
            string keyFile = GetOfflineKeyFilePath();
            if (!File.Exists(keyFile))
                return new List<string>();
            return File.ReadAllLines(keyFile)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        private void WriteOfflineUsernames(List<string> usernames)
        {
            string keyFile = GetOfflineKeyFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(keyFile));
            File.WriteAllLines(keyFile, usernames);
        }
        private void Offline_RemoveClicked(string username)
        {
            var usernames = ReadOfflineUsernames();
            usernames.RemoveAll(u => u.Equals(username, StringComparison.OrdinalIgnoreCase));
            WriteOfflineUsernames(usernames);
            RefreshUI();
        }
        private void Offline_LoginClicked(string username)
        {
            ShowAutoLoginPanel();
            var session = MSession.CreateOfflineSession(username);
            _autoLoginDone = true;
            OpenMainRequested?.Invoke(session);
            _hidePanelTimer.Start();
        }
        private void HidePanelTimer_Tick(object sender, EventArgs e)
        {
            _hidePanelTimer.Stop();
            HideAutoLoginPanel();
        }
        private async void AccountManager_Load(object sender, EventArgs e)
        {
            RefreshUI();
            Console.WriteLine("AccountManager_Load called");
            if (_isAuthenticating)
                return;
            _isAuthenticating = true;
            await AutoLoginAsync();
            RefreshUI();
            _isAuthenticating = false;
            Console.WriteLine($"Accounts loaded: {_cachedAccounts.Count}");
        }
        private void CreateAutoLoginPanel()
        {
            _autoLoginPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(200, 20, 20, 20),
                Visible = false
            };
            _autoLoginLabel = new Label
            {
                Text = LanguageManager.Get("AccountManager.GettingReady"),
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            _autoLoginaltLabel = new Label
            {
                Text = LanguageManager.Get("AccountManager.PleaseWait"),
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.Transparent
            };
            _autoLoginPanel.Controls.Add(_autoLoginaltLabel);
            _autoLoginPanel.Controls.Add(_autoLoginLabel);
            _autoLoginPanel.Resize += (s, e) =>
            {
                CenterLabelsInPanel();
            };
            this.Controls.Add(_autoLoginPanel);
            _autoLoginPanel.BringToFront();
        }
        private void CenterLabelsInPanel()
        {
            if (_autoLoginPanel == null || _autoLoginLabel == null || _autoLoginaltLabel == null)
                return;
            int totalHeight = _autoLoginLabel.Height + 5 + _autoLoginaltLabel.Height;
            int startY = (_autoLoginPanel.Height - totalHeight) / 2;
            _autoLoginLabel.Location = new Point(
                (_autoLoginPanel.Width - _autoLoginLabel.Width) / 2,
                startY
            );
            _autoLoginaltLabel.Location = new Point(
                (_autoLoginPanel.Width - _autoLoginaltLabel.Width) / 2,
                _autoLoginLabel.Bottom + 5
            );
        }
        private void ShowAutoLoginPanel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowAutoLoginPanel));
                return;
            }
            if (_autoLoginPanel == null) return;
            _autoLoginPanel.Visible = true;
            _autoLoginPanel.BringToFront();
            CenterLabelsInPanel();
        }
        private void HideAutoLoginPanel()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HideAutoLoginPanel));
                return;
            }
            if (_autoLoginPanel == null) return;
            _autoLoginPanel.Visible = false;
        }
        private async Task AutoLoginAsync()
        {
            if (_autoLoginDone)
            {
                HideAutoLoginPanel();
                return;
            }
            ShowAutoLoginPanel();
            try
            {
                _cachedAccounts = _loginHandler.AccountManager
                    .GetAccounts()
                    .OfType<JEGameAccount>()
                    .ToList();
                if (_cachedAccounts.Count == 0)
                {
                    var offlineUsernames = ReadOfflineUsernames();
                    if (offlineUsernames.Count > 0)
                    {
                        string username = offlineUsernames.First();
                        var offlineSession = MSession.CreateOfflineSession(username);
                        _autoLoginDone = true;
                        OpenMainRequested?.Invoke(offlineSession);
                        _hidePanelTimer.Start();
                        return;
                    }
                    HideAutoLoginPanel();
                    return;
                }
                string lastId = Properties.Settings.Default.LastAccountId;
                JEGameAccount lastAccount = null;
                if (!string.IsNullOrEmpty(lastId))
                {
                    lastAccount = _cachedAccounts.FirstOrDefault(x => x.Identifier == lastId);
                }
                lastAccount ??= _cachedAccounts.First();
                var session = await Task.Run(() =>
                    _loginHandler.Authenticate(lastAccount)
                );
                _autoLoginDone = true;
                OpenMainRequested?.Invoke(session);
                _hidePanelTimer.Start();
            }
            catch (Exception ex)
            {
                HideAutoLoginPanel();
                Console.WriteLine($"Auto login failed: {ex.Message}");
                RefreshUI();
            }
        }
        public void RefreshUI()
        {
            label1.Anchor = AnchorStyles.Top;
            if (_isAuthenticating)
                return;
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshUI));
                return;
            }
            try
            {
                Console.WriteLine("RefreshUI called");
                if (flowAccounts == null)
                {
                    Console.WriteLine("flowAccounts is null!");
                    return;
                }
                flowAccounts.Controls.Clear();
                var accounts = _loginHandler.AccountManager
                    .GetAccounts()
                    .OfType<JEGameAccount>()
                    .ToList();
                _cachedAccounts = accounts;
                Console.WriteLine($"Found {accounts.Count} accounts");
                foreach (var acc in accounts)
                {
                    var item = new AccountControl(acc);
                    item.LoginClicked += Account_LoginClicked;
                    item.RemoveClicked += Account_RemoveClicked;
                    flowAccounts.Controls.Add(item);
                    Console.WriteLine($"Added account: {acc.Profile?.Username}");
                }
                var offlineUsernames = ReadOfflineUsernames();
                foreach (var username in offlineUsernames)
                {
                    var offlineItem = new AccountControl(username);
                    offlineItem.OfflineLoginClicked += Offline_LoginClicked;
                    offlineItem.OfflineRemoveClicked += () => Offline_RemoveClicked(username);
                    flowAccounts.Controls.Add(offlineItem);
                    Console.WriteLine($"Added offline account: {username}");
                }
                if (flowAccounts.Controls.Count == 0)
                {
                    Label noAccountLabel = new Label
                    {
                        Text = LanguageManager.Get("AccountManager.NoAccounts"),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 12, FontStyle.Regular),
                        AutoSize = true,
                        Location = new Point(20, 20)
                    };
                    flowAccounts.Controls.Add(noAccountLabel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refresh error: {ex.Message}");
                MessageBox.Show(
                    LanguageManager.GetFormatted("AccountManager.RefreshError", ex.Message),
                    LanguageManager.Get("Main.ErrorTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private async void Account_LoginClicked(object? sender, JEGameAccount account)
        {
            if (account == null || _isAuthenticating)
                return;
            _isAuthenticating = true;
            ShowAutoLoginPanel();
            try
            {
                var session = await Task.Run(() =>
                    _loginHandler.Authenticate(account)
                );
                Properties.Settings.Default.LastAccountId = account.Identifier;
                Properties.Settings.Default.Save();
                _autoLoginDone = true;
                OpenMainRequested?.Invoke(session);
                _hidePanelTimer.Start();
            }
            catch (Exception ex)
            {
                HideAutoLoginPanel();
                Debug.Print(ex.ToString());
            }
            finally
            {
                _isAuthenticating = false;
            }
        }
        private void Account_RemoveClicked(object? sender, JEGameAccount account)
        {
            if (account == null) return;
            try
            {
                _loginHandler.Signout(account);
                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageManager.GetFormatted("AccountManager.RemoveFailed", ex.Message),
                    LanguageManager.Get("Main.ErrorTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private async void button1_Click_1(object sender, EventArgs e)
        {
            panel1.Show();
        }
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        private void ControlPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                this.FindForm()?.BeginInvoke(new Action(() =>
                {
                    SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }));
            }
        }
        private void ControlPanel_Paint(object sender, PaintEventArgs e) { }
        private void flowAccounts_Paint(object sender, PaintEventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private async void button2_Click(object sender, EventArgs e)
        {
            if (_isAuthenticating)
                return;
            button1.Enabled = false;
            _isAuthenticating = true;
            ShowAutoLoginPanel();
            try
            {
                await _loginHandler.AuthenticateInteractively();
                HideAutoLoginPanel();
            }
            catch (Exception ex)
            {
                HideAutoLoginPanel();
                Debug.Print(ex.ToString());
            }
            finally
            {
                _isAuthenticating = false;
                button1.Enabled = true;
                RefreshUI();
            }
            panel1.Hide();
        }
        public void ShowOffline()
        {
            isOfflineMode = true;
            label1.Text = LanguageManager.Get("AccountManager.OfflineTitle");
            label2.Text = LanguageManager.Get("AccountManager.OfflineMode");
            guna2TextBox1.Show();
            label3.Show();
            button3.Text = LanguageManager.Get("AccountManager.OfflineSave");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (isOfflineMode)
            {
                try
                {
                    string username = guna2TextBox1.Text.Trim();
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.UsernameEmpty"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (username.Length < 3)
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.UsernameShort"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (username.Length > 16)
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.UsernameLong"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(username, "^[A-Za-z0-9_]+$"))
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.UsernameInvalid"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    if (username.Equals("Ywix", StringComparison.OrdinalIgnoreCase) ||
                        username.Equals("FixzyXqw", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.CannotBeOfflineAsDeveloper"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    var offlineUsernames = ReadOfflineUsernames();
                    if (offlineUsernames.Any(u => u.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show(
                            LanguageManager.Get("AccountManager.UsernameAlreadyExists"),
                            LanguageManager.Get("Main.ErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }
                    offlineUsernames.Add(username);
                    WriteOfflineUsernames(offlineUsernames);
                    var session = MSession.CreateOfflineSession(username);
                    _autoLoginDone = true;
                    panel1.Hide();
                    OpenMainRequested?.Invoke(session);
                    _hidePanelTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        LanguageManager.GetFormatted("AccountManager.KeyFileError", ex.Message),
                        LanguageManager.Get("Main.ErrorTitle"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            else
            {
                ShowOffline();
            }
        }
            private void panel1_Paint(object sender, PaintEventArgs e)
        {
            var rect = panel1.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            int radius = 16;
            using var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            panel1.Region = new Region(path);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(45, 45, 48), 1.5f);
            e.Graphics.DrawPath(pen, path);
        }
        private void label3_Click(object sender, EventArgs e)
        {
        }
        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Hide();
        }
    }
}

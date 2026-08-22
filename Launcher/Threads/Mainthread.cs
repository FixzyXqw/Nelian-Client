using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using XboxAuthNet.Game;
using XboxAuthNet.Game.Accounts;

namespace Nelian
{
    public partial class MainThread : Form
    {
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTCAPTION = 2;
        private const int HTCLIENT = 1;

        private JELoginHandler loginHandler = JELoginHandlerBuilder.BuildDefault();

        private Panel titleBar;
        private Label lblTitle;
        private HoverButton btnClose;
        private HoverButton btnMinimize;
        private RoundedSearchBox searchBox;
        private UpdateIcon updateIcon;
        private ToolTip updateToolTip;

        private Panel contentPanel;

        private UserControl? _currentControl;
        private UserControl? _nextControl;
        private bool _isTransitioning = false;
        private bool _isLoading = false;

        private readonly Color BackDark = Color.FromArgb(24, 24, 24);
        private readonly Color TitleBarColor = Color.FromArgb(30, 30, 30);
        private readonly Color AccentGreen = Color.FromArgb(85, 170, 85);

        private System.Windows.Forms.Timer _updateTimer;
        private bool _isUpdating = false;
        private UpdateCheckResult _lastUpdateResult = UpdateCheckResult.NoUpdate;

        private System.Windows.Forms.Timer _updatePanelTimer;
        private DateTime _updatePanelAnimStart;
        private float _updatePanelFromX;
        private float _updatePanelToX;
        private const int UPDATE_PANEL_ANIM_MS = 260;
        private bool _isUpdatePanelAnimating = false;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int attr,
            ref int attrValue,
            int attrSize);

        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            Default = 0,
            DoNotRound = 1,
            Round = 2,
            RoundSmall = 3
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;

        public MainThread()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = BackDark;
            Size = new Size(961, 550);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            InitTitleBar();
            InitContentPanel();

            this.SuspendLayout();
        }

        private void InitContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BackDark
            };
            this.Controls.Add(contentPanel);
            contentPanel.BringToFront();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCCALCSIZE:
                    if (m.WParam != IntPtr.Zero)
                    {
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    break;

                case WM_NCHITTEST:
                    Point p = PointToClient(new Point(
                        (short)(m.LParam.ToInt32() & 0xFFFF),
                        (short)(m.LParam.ToInt32() >> 16)));

                    if (updateIcon != null && updateIcon.Visible && updateIcon.Bounds.Contains(p))
                    {
                        m.Result = (IntPtr)HTCLIENT;
                        return;
                    }

                    if (p.Y < 40)
                    {
                        m.Result = (IntPtr)HTCAPTION;
                        return;
                    }

                    base.WndProc(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        private void Nelian_Load(object sender, EventArgs e)
        {
            int preference = (int)DWM_WINDOW_CORNER_PREFERENCE.Round;
            DwmSetWindowAttribute(
                Handle,
                DWMWA_WINDOW_CORNER_PREFERENCE,
                ref preference,
                sizeof(int));

            this.ResumeLayout(true);
            this.PerformLayout();

            ShowSplash();
        }

        private void OnBackToMain(object? sender, EventArgs e)
        {
            var main = new Main();
            main.SetSession(Main._session);
            main.GoAccountManagerClicked += OnGoAccountManager;
            main.OpenModrinthRequested += OnOpenModrinth;
            PreloadAndSetControl(main);
        }

        private void ShowModrinth()
        {
            var modrinth = new ModrinthMain();
            PreloadAndSetControl(modrinth);
        }

        private void ShowSplash()
        {
            updateIcon.Visible = false;
            var splash = new Splash();
            splash.LoadingFinished += OnSplashFinished;
            PreloadAndSetControl(splash);
        }

        private void OnSplashFinished()
        {
            updateIcon.Visible = true;
            if (InvokeRequired)
            {
                Invoke(new Action(ShowAccountManager));
            }
            else
            {
                ShowAccountManager();
            }
            StartUpdateTimer();
        }

        private void ShowAccountManager()
        {
            var accountManager = new AccountManager(loginHandler);
            accountManager.OpenMainRequested += OnMainRequested;
            PreloadAndSetControl(accountManager);
        }

        private void OnOpenModrinth(object? sender, EventArgs e)
        {
            var modrinth = new ModrinthMain();
            modrinth.BackRequested += OnBackToMain;
            PreloadAndSetControl(modrinth);
        }

        private void OnMainRequested(MSession session)
        {
            var main = new Main();
            main.SetSession(session);
            main.GoAccountManagerClicked += OnGoAccountManager;
            main.OpenModrinthRequested += OnOpenModrinth;
            PreloadAndSetControl(main);
        }

        private void OnGoAccountManager(object? sender, EventArgs e)
        {
            ShowAccountManager();
        }

        private async void PreloadAndSetControl(UserControl control)
        {
            if (_isTransitioning || _isLoading)
                return;

            _isLoading = true;

            try
            {
                control.Dock = DockStyle.Fill;
                control.Visible = false;
                contentPanel.Controls.Add(control);
                control.BringToFront();

                var tcs = new TaskCompletionSource<bool>();
                EventHandler loadHandler = null;
                loadHandler = (s, ev) =>
                {
                    control.Load -= loadHandler;
                    tcs.TrySetResult(true);
                };

                if (!control.IsHandleCreated)
                {
                    control.Load += loadHandler;
                }

                control.CreateControl();
                foreach (Control child in control.Controls)
                {
                    child.CreateControl();
                }
                control.PerformLayout();

                await Task.Delay(100);

                SetControl(control);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Preload error: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void SetControl(UserControl control)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<UserControl>(SetControl), control);
                return;
            }

            if (_isTransitioning)
                return;

            _isTransitioning = true;

            contentPanel.SuspendLayout();
            this.SuspendLayout();

            if (_currentControl != null)
            {
                if (_currentControl is Splash splash)
                    splash.LoadingFinished -= OnSplashFinished;
                else if (_currentControl is AccountManager accountManager)
                    accountManager.OpenMainRequested -= OnMainRequested;
                else if (_currentControl is Main main)
                    main.GoAccountManagerClicked -= OnGoAccountManager;

                contentPanel.Controls.Remove(_currentControl);
                _currentControl.Dispose();
                _currentControl = null;
            }

            _currentControl = control;
            _currentControl.Visible = true;
            _currentControl.BringToFront();

            contentPanel.ResumeLayout(true);
            contentPanel.PerformLayout();
            this.ResumeLayout(true);
            this.PerformLayout();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            _isTransitioning = false;
        }

        private void InitTitleBar()
        {
            titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = TitleBarColor
            };
            this.Controls.Add(titleBar);
            titleBar.BringToFront();
            EnableDrag(titleBar);

            lblTitle = new Label
            {
                Text = "Nelian",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(14, 10),
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblTitle);
            EnableDrag(lblTitle);

            updateIcon = new UpdateIcon
            {
                Size = new Size(24, 24),
                Location = new Point(lblTitle.Right + 2, 8),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            updateIcon.Click += UpdateIcon_Click;
            titleBar.Controls.Add(updateIcon);

            updateToolTip = new ToolTip();
            updateToolTip.SetToolTip(updateIcon, LanguageManager.Get("Main.UpdateChecking"));

            btnClose = new HoverButton("\u2715", TitleBarColor, Color.FromArgb(232, 17, 35))
            {
                Size = new Size(40, 40),
                Location = new Point(titleBar.Width - 40, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => Close();
            titleBar.Controls.Add(btnClose);

            btnMinimize = new HoverButton("\u2014", TitleBarColor, Color.FromArgb(70, 70, 70))
            {
                Size = new Size(40, 40),
                Location = new Point(titleBar.Width - 80, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnMinimize.Click += (s, e) => WindowState = FormWindowState.Minimized;
            titleBar.Controls.Add(btnMinimize);

            searchBox = new RoundedSearchBox
            {
                Size = new Size(260, 28),
                BackColor = TitleBarColor
            };
            titleBar.Controls.Add(searchBox);
            CenterSearchBox();

            titleBar.Resize += (s, e) =>
            {
                btnClose.Location = new Point(titleBar.Width - 40, 0);
                btnMinimize.Location = new Point(titleBar.Width - 80, 0);
                CenterSearchBox();
                updateIcon.Location = new Point(lblTitle.Right + 2, 8);
            };

            lblTitle.TextChanged += (s, e) =>
            {
                updateIcon.Location = new Point(lblTitle.Right + 2, 8);
            };
        }

        private void CenterSearchBox()
        {
            searchBox.Location = new Point(
                (titleBar.Width - searchBox.Width) / 2,
                (titleBar.Height - searchBox.Height) / 2);
        }

        private void EnableDrag(Control c)
        {
            c.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            };
        }

        private void AnimateUpdatePanel(bool show)
        {
            if (_currentControl is not Main main)
                return;

            var panel = main.UpdateMessagePanel;
            if (panel == null || panel.IsDisposed)
                return;

            if (show)
            {
                if (_isUpdatePanelAnimating)
                    return;

                if (panel.Visible && panel.Location.X == -5)
                    return;

                var label1 = panel.Controls.Find("UpdateLabel1", true).FirstOrDefault() as Label;
                var label2 = panel.Controls.Find("UpdateLabel2", true).FirstOrDefault() as Label;

                if (label1 != null)
                {
                    label1.Text = LanguageManager.Get("Main.OutDate");
                    label1.Visible = true;
                }

                if (label2 != null)
                {
                    label2.Text = LanguageManager.Get("Main.ClickToInstall");
                    label2.Visible = true;
                }

                panel.Visible = true;
                panel.BringToFront();

                int startX = -panel.Width;
                int endX = -5;

                panel.Location = new Point(startX, 10);

                _updatePanelFromX = startX;
                _updatePanelToX = endX;
                _updatePanelAnimStart = DateTime.Now;
                _isUpdatePanelAnimating = true;

                if (_updatePanelTimer == null)
                {
                    _updatePanelTimer = new System.Windows.Forms.Timer { Interval = 12 };
                    _updatePanelTimer.Tick += UpdatePanelTimer_Tick;
                }
                _updatePanelTimer.Start();
            }
            else
            {
                panel.Visible = false;
                _isUpdatePanelAnimating = false;
                _updatePanelTimer?.Stop();
            }
        }

        private void UpdatePanelTimer_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - _updatePanelAnimStart).TotalMilliseconds;
            float t = (float)Math.Min(1.0, elapsed / UPDATE_PANEL_ANIM_MS);
            float eased = 1 - (float)Math.Pow(1 - t, 3);

            float x = _updatePanelFromX + (_updatePanelToX - _updatePanelFromX) * eased;

            if (_currentControl is Main main && main.UpdateMessagePanel != null)
            {
                main.UpdateMessagePanel.Location = new Point((int)x, 10);
            }

            if (t >= 1f)
            {
                _updatePanelTimer.Stop();
                _isUpdatePanelAnimating = false;

                if (_currentControl is Main main2 && main2.UpdateMessagePanel != null)
                {
                    main2.UpdateMessagePanel.Location = new Point((int)_updatePanelToX, 10);
                }
            }
        }

        private async void UpdateIcon_Click(object sender, EventArgs e)
        {
            await PerformUpdateCheck();

            if (_lastUpdateResult == UpdateCheckResult.LauncherUpdate)
            {
                UpdateManager.ApplyLauncherUpdate();
            }
            else if (_lastUpdateResult == UpdateCheckResult.ClientUpdate)
            {
                try
                {
                    updateIcon.Enabled = false;
                    updateToolTip.SetToolTip(updateIcon, LanguageManager.Get("Main.UpdateApplying"));

                    if (_currentControl is Main main)
                    {
                        main.SetLaunchButtonsEnabled(false);
                    }

                    bool success = await UpdateManager.ApplyClientUpdateAsync();

                    if (success)
                    {
                        _lastUpdateResult = UpdateCheckResult.NoUpdate;
                        updateToolTip.SetToolTip(updateIcon, LanguageManager.Get("Main.UpdateNoUpdate"));
                        AnimateUpdatePanel(false);
                    }
                }
                catch (Exception ex)
                {
                    updateToolTip.SetToolTip(updateIcon, string.Format(LanguageManager.Get("Main.UpdateCheckError"), ex.Message));
                    MessageBox.Show(
                        string.Format(LanguageManager.Get("Main.UpdateCheckError"), ex.Message),
                        LanguageManager.Get("Main.ErrorTitle"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    updateIcon.Enabled = true;

                    if (_currentControl is Main main)
                    {
                        main.SetLaunchButtonsEnabled(true);
                    }
                }
            }
            else
            {
                updateToolTip.SetToolTip(updateIcon, LanguageManager.Get("Main.UpdateNoUpdate"));
                AnimateUpdatePanel(false);
            }
        }

        private void StartUpdateTimer()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new System.Windows.Forms.Timer();
                _updateTimer.Interval = 30 * 60 * 1000;
                _updateTimer.Tick += UpdateTimer_Tick;
            }
            _updateTimer.Start();

            Task.Delay(5000).ContinueWith(_ =>
            {
                if (!IsDisposed)
                    BeginInvoke(new Action(async () => await PerformUpdateCheck()));
            });
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            await PerformUpdateCheck();
        }

        private async Task PerformUpdateCheck()
        {
            if (_isUpdating)
                return;

            _isUpdating = true;
            try
            {
                updateIcon.SetChecking(true);
                updateToolTip.SetToolTip(updateIcon, LanguageManager.Get("Main.UpdateChecking"));

                var result = await UpdateManager.CheckAllUpdatesAsync();
                _lastUpdateResult = result;

                bool hasUpdate = result == UpdateCheckResult.LauncherUpdate ||
                                 result == UpdateCheckResult.ClientUpdate;

                updateIcon.SetHasUpdate(hasUpdate);

                string tooltipText;
                if (hasUpdate)
                    tooltipText = LanguageManager.Get("Main.UpdateAvailable");
                else
                    tooltipText = LanguageManager.Get("Main.UpdateNoUpdate");

                updateToolTip.SetToolTip(updateIcon, tooltipText);

                if (hasUpdate)
                {
                    AnimateUpdatePanel(true);
                }
                else
                {
                    AnimateUpdatePanel(false);
                }
            }
            catch
            {
            }
            finally
            {
                updateIcon.SetChecking(false);
                _isUpdating = false;
            }
        }

        private void Client_Paint(object sender, PaintEventArgs e) { }
        private void ControlPanel_Paint(object sender, PaintEventArgs e) { }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (titleBar != null)
            {
                btnClose.Location = new Point(titleBar.Width - 40, 0);
                btnMinimize.Location = new Point(titleBar.Width - 80, 0);
                CenterSearchBox();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _updateTimer?.Stop();
            _updateTimer?.Dispose();
            _updatePanelTimer?.Stop();
            _updatePanelTimer?.Dispose();

            if (_currentControl != null)
            {
                if (_currentControl is Splash splash)
                    splash.LoadingFinished -= OnSplashFinished;
                else if (_currentControl is AccountManager accountManager)
                    accountManager.OpenMainRequested -= OnMainRequested;
                else if (_currentControl is Main main)
                    main.GoAccountManagerClicked -= OnGoAccountManager;

                contentPanel.Controls.Remove(_currentControl);
                _currentControl.Dispose();
                _currentControl = null;
            }

            if (_nextControl != null)
            {
                _nextControl.Dispose();
                _nextControl = null;
            }

            base.OnFormClosing(e);
        }
    }

    public class UpdateIcon : Control
    {
        private const int ArrowSize = 12;
        private Color _iconColor = Color.FromArgb(120, 120, 120);
        private Color _targetColor = Color.FromArgb(120, 120, 120);
        private Color _hoverColor = Color.FromArgb(160, 160, 160);
        private Color _activeColor = Color.FromArgb(85, 170, 85);

        private bool _isChecking = false;
        private float _pulsePhase = 0;
        private System.Windows.Forms.Timer _pulseTimer;
        private bool _hasUpdate = false;

        private float _currentFade = 0;
        private float _targetFade = 0;
        private System.Windows.Forms.Timer _fadeTimer;

        public UpdateIcon()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();

            BackColor = Color.Transparent;

            _pulseTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _pulseTimer.Tick += PulseTimer_Tick;

            _fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _fadeTimer.Tick += FadeTimer_Tick;
        }

        public void SetChecking(bool checking)
        {
            _isChecking = checking;
            if (checking)
            {
                _pulseTimer.Start();
            }
            else
            {
                _pulseTimer.Stop();
                _pulsePhase = 0;
                Invalidate();
            }
        }

        public void SetHasUpdate(bool hasUpdate)
        {
            _hasUpdate = hasUpdate;
            _targetFade = hasUpdate ? 1f : 0f;

            if (hasUpdate)
            {
                _targetColor = _activeColor;
                _hoverColor = Color.FromArgb(120, 200, 120);
            }
            else
            {
                _targetColor = Color.FromArgb(120, 120, 120);
                _hoverColor = Color.FromArgb(160, 160, 160);
            }

            if (!_fadeTimer.Enabled)
                _fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(_currentFade - _targetFade) < 0.01f)
            {
                _currentFade = _targetFade;
                _fadeTimer.Stop();
            }
            else
            {
                _currentFade += (_targetFade - _currentFade) * 0.08f;
            }

            _iconColor = LerpColor(Color.FromArgb(120, 120, 120), _activeColor, _currentFade);
            Invalidate();
        }

        private void PulseTimer_Tick(object sender, EventArgs e)
        {
            _pulsePhase += 0.05f;
            if (_pulsePhase > Math.PI * 2)
                _pulsePhase -= (float)(Math.PI * 2);
            Invalidate();
        }

        private Color LerpColor(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color currentColor = this.ClientRectangle.Contains(PointToClient(MousePosition)) ? _hoverColor : _iconColor;

            using (var pen = new Pen(currentColor, 2))
            {
                int cx = Width / 2;
                int cy = Height / 2;
                int half = ArrowSize / 2;

                e.Graphics.DrawLine(pen, cx, cy - half, cx, cy + half - 2);
                e.Graphics.DrawLine(pen, cx - half + 2, cy - 2, cx, cy + half - 2);
                e.Graphics.DrawLine(pen, cx + half - 2, cy - 2, cx, cy + half - 2);
                e.Graphics.DrawLine(pen, cx - half + 2, cy + half, cx + half - 2, cy + half);
            }

            if (_isChecking)
            {
                float pulseSize = 3 + (float)Math.Sin(_pulsePhase) * 1.5f;
                float alpha = 100 + (float)Math.Sin(_pulsePhase + Math.PI / 2) * 50;

                using (var brush = new SolidBrush(Color.FromArgb((int)alpha, 85, 170, 85)))
                {
                    int dotX = Width - 6;
                    int dotY = 4;
                    e.Graphics.FillEllipse(brush, dotX - pulseSize / 2, dotY - pulseSize / 2, pulseSize, pulseSize);
                }
            }

            if (_hasUpdate)
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 85, 170, 85)))
                {
                    int dotX = Width - 5;
                    int dotY = 5;
                    e.Graphics.FillEllipse(brush, dotX - 3, dotY - 3, 6, 6);
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pulseTimer?.Stop();
                _pulseTimer?.Dispose();
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class HoverButton : Control
    {
        private readonly Color normalColor;
        private readonly Color hoverColor;
        private Color currentColor;
        private readonly System.Windows.Forms.Timer animTimer;
        private int step = 0;
        private const int steps = 6;
        private bool hovering = false;
        private readonly string glyph;

        public HoverButton(string glyph, Color normalColor, Color hoverColor)
        {
            this.glyph = glyph;
            this.normalColor = normalColor;
            this.hoverColor = hoverColor;
            currentColor = normalColor;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            BackColor = normalColor;
            Cursor = Cursors.Hand;

            animTimer = new System.Windows.Forms.Timer { Interval = 12 };
            animTimer.Tick += AnimTimer_Tick;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            hovering = true;
            animTimer.Start();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            hovering = false;
            animTimer.Start();
            base.OnMouseLeave(e);
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            step += hovering ? 1 : -1;
            step = Math.Max(0, Math.Min(steps, step));

            float t = (float)step / steps;
            currentColor = Lerp(normalColor, hoverColor, t);
            Invalidate();

            if ((hovering && step == steps) || (!hovering && step == 0))
                animTimer.Stop();
        }

        private static Color Lerp(Color a, Color b, float t)
        {
            return Color.FromArgb(
                255,
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t)
            );
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(currentColor);

            using (var textBrush = new SolidBrush(Color.White))
            using (var font = new Font("Segoe UI", 10F))
            {
                var size = e.Graphics.MeasureString(glyph, font);
                e.Graphics.DrawString(glyph, font, textBrush,
                    (Width - size.Width) / 2, (Height - size.Height) / 2);
            }
        }
    }

    public class RoundedSearchBox : Panel
    {
        private readonly TextBox textBox;
        private readonly int radius = 14;

        public string PlaceholderText { get; set; } = "Search...";

        public RoundedSearchBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            Padding = new Padding(30, 4, 8, 4);

            textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                Dock = DockStyle.Fill,
                TextAlign = HorizontalAlignment.Center
            };
            textBox.GotFocus += (s, e) => Invalidate();
            textBox.LostFocus += (s, e) => Invalidate();
            textBox.TextChanged += (s, e) => Invalidate();
            Controls.Add(textBox);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = new Region(RoundedPath(ClientRectangle, radius));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var bg = new SolidBrush(Color.FromArgb(45, 45, 45)))
                e.Graphics.FillPath(bg, RoundedPath(ClientRectangle, radius));

            Color borderColor = textBox.Focused
                ? Color.FromArgb(85, 170, 85)
                : Color.FromArgb(70, 70, 70);

            using (var pen = new Pen(borderColor, 1.3f))
                e.Graphics.DrawPath(pen, RoundedPath(Rectangle.Inflate(ClientRectangle, -1, -1), radius));

            using (var pen = new Pen(Color.FromArgb(180, 180, 180), 1.6f))
            {
                var circle = new Rectangle(9, Height / 2 - 6, 10, 10);
                e.Graphics.DrawEllipse(pen, circle);
                e.Graphics.DrawLine(pen, circle.Right - 1, circle.Bottom - 1, circle.Right + 4, circle.Bottom + 4);
            }

            if (string.IsNullOrEmpty(textBox.Text) && !textBox.Focused)
            {
                using (var placeholderBrush = new SolidBrush(Color.FromArgb(140, 140, 140)))
                using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.DrawString(PlaceholderText, textBox.Font, placeholderBrush, textBox.Bounds, format);
                }
            }
        }

        private static GraphicsPath RoundedPath(Rectangle rect, int r)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

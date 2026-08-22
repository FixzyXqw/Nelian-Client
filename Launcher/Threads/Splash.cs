using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nelian
{
    public partial class Splash : UserControl
    {
        private class DownloadItem
        {
            public string Url { get; set; }
            public string TempFileName { get; set; }
            public string DestRelativePath { get; set; }
            public string MinecraftDestRelativePath { get; set; }
            public string JavaDestRelativePath { get; set; }
            public string ProgramFilesDestRelativePath { get; set; }
            public bool IsZip { get; set; }
        }

        private readonly DownloadItem[] downloads = new DownloadItem[]
        {
            new DownloadItem
            {
                Url = "https://github.com/FixzyXqw/Nelian-Client/releases/download/NelianClientUpdate/Nelian.jar",
                TempFileName = "Secure.tmp",
                DestRelativePath = @"Nelian.runtime",
                IsZip = false
            },
            new DownloadItem
            {
                Url = "https://piston-data.mojang.com/v1/objects/0983f08be6a4e624f5d85689d1aca869ed99c738/client.jar",
                TempFileName = "Client.tmp",
                MinecraftDestRelativePath = @"versions\1.8.8\1.8.8.jar",
                IsZip = false
            },
            new DownloadItem
            {
                Url = "https://github.com/FixzyXqw/Nelian-Client/releases/download/Assets/jdk-8.0.492.9-hotspot.zip",
                TempFileName = "jdk-8.0.492.9-hotspot.tmp",
                JavaDestRelativePath = @"jdk-8.0.492.9-hotspot",
                IsZip = true
            },
            new DownloadItem
            {
                Url = "https://github.com/FixzyXqw/Nelian-Client/releases/download/NelianClientUpdate/NelianGuard.dll",
                TempFileName = "NelianGuard.tmp",
                ProgramFilesDestRelativePath = @"NelianGuard.dll",
                IsZip = false
            }
        };

        private string appDataRoot;
        private string tempFolder;
        private string MinecraftRoot;
        private string JavaRoot;
        private string ProgramFilesRoot;
        public event Action LoadingFinished;

        private string expectedHash;
        private bool needsUpdate = false;

        private string configFilePath;

        private System.Windows.Forms.Timer _smoothTimer;
        private float _currentProgress = 0;
        private float _targetProgress = 0;
        private const float SMOOTH_SPEED = 0.15f;

        public Splash()
        {
            InitializeComponent();

            LoadingBar.Minimum = 0;
            LoadingBar.Maximum = 100;
            LoadingBar.Value = 0;
            LoadingBar.Text = "0%";
            label1.Text = LanguageManager.Get("Splash.CheckingUpdates");

            UpdateManager.ProgressChanged += OnUpdateProgressChanged;

            _smoothTimer = new System.Windows.Forms.Timer();
            _smoothTimer.Interval = 16;
            _smoothTimer.Tick += SmoothTimer_Tick;
            _smoothTimer.Start();
        }

        private void SmoothTimer_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(_currentProgress - _targetProgress) < 0.5f)
            {
                _currentProgress = _targetProgress;
            }
            else
            {
                _currentProgress += (_targetProgress - _currentProgress) * SMOOTH_SPEED;
            }

            int displayValue = (int)Math.Round(_currentProgress);
            displayValue = Math.Max(0, Math.Min(100, displayValue));

            LoadingBar.Value = displayValue;
            LoadingBar.Text = $"{displayValue}%";
        }

        private void OnUpdateProgressChanged(int progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(OnUpdateProgressChanged), progress);
                return;
            }

            _targetProgress = Math.Max(0, Math.Min(100, progress));
        }

        private void CenterControls()
        {
            CenterLabel();
            LoadingBar.Left = (this.ClientSize.Width - LoadingBar.Width) / 2;
            pictureBox1.Left = label1.Left + (label1.Width - pictureBox1.Width) / 2;
            pictureBox1.Top = label1.Top - pictureBox1.Height - 25;
        }

        private void CenterPictureBox()
        {
            pictureBox1.Location = new Point(
                (ClientSize.Width - pictureBox1.Width) / 2 + 30,
                (ClientSize.Height - pictureBox1.Height) / 2 - 50
            );
        }

        private void CenterLabel()
        {
            label1.AutoSize = true;
            label1.PerformLayout();
            label1.Left = pictureBox1.Left + (pictureBox1.Width - label1.Width) / 2;
        }

        private readonly Color bgBase = Color.FromArgb(14, 14, 14);
        private readonly Color neonBlue = Color.FromArgb(0, 170, 255);
        private readonly Color textTarget = Color.FromArgb(225, 225, 225);

        private void Splash_Load(object sender, EventArgs e)
        {
            label1.ForeColor = Color.White;
            CenterPictureBox();
            int scaling = (int)Math.Round(DeviceDpi / 96.0 * 100);
            StartLoading();
        }

        public void StartLoading()
        {
            EnsureFolders();
            _ = ProcessDownloads();
        }

        private void EnsureFolders()
        {
            ProgramFilesRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Nelian"
            );

            JavaRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft", "JRE"
            );

            MinecraftRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ".minecraft"
            );

            appDataRoot = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Microsoft", "Game", "Nelian"
            );

            tempFolder = Path.Combine(appDataRoot, "temp");
            configFilePath = Path.Combine(MinecraftRoot, "NelianLauncherProperties.txt");

            try
            {
                Directory.CreateDirectory(ProgramFilesRoot);
                Directory.CreateDirectory(appDataRoot);
                Directory.CreateDirectory(tempFolder);
                Directory.CreateDirectory(Path.Combine(appDataRoot, "versions", "1.8.8"));
                Directory.CreateDirectory(JavaRoot);
                Directory.CreateDirectory(MinecraftRoot);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LanguageManager.Get("Splash.FolderCreateError"), ex.Message),
                    LanguageManager.Get("Splash.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            if (!File.Exists(configFilePath))
            {
                File.WriteAllText(configFilePath,
@"Blur=0
Memory=4096
LiveAnimations=0
Theme=Newgen");
            }
        }

        private bool IsLiveAnimationsEnabled()
        {
            if (!File.Exists(configFilePath))
                return false;

            foreach (string line in File.ReadAllLines(configFilePath))
            {
                if (line.StartsWith("LiveAnimations="))
                    return line.Substring("LiveAnimations=".Length).Trim() == "1";
            }

            return false;
        }

        private string GetConfigValue(string key)
        {
            if (!File.Exists(configFilePath))
                return null;

            foreach (string line in File.ReadAllLines(configFilePath))
            {
                if (line.StartsWith(key + "="))
                    return line.Substring(key.Length + 1).Trim();
            }
            return null;
        }

        private async Task ProcessDownloads()
        {
            try
            {
                UpdateStatus(LanguageManager.Get("Splash.CheckingUpdates"));

                bool launcherNeedsUpdate = await UpdateManager.CheckLauncherUpdateAsync();
                needsUpdate = launcherNeedsUpdate;

                if (launcherNeedsUpdate)
                {
                    UpdateStatus(LanguageManager.Get("Splash.InstallingUpdates"));
                    UpdateManager.ApplyLauncherUpdate();
                    return;
                }

                UpdateStatus(LanguageManager.Get("Splash.CheckingUpdates"));

                bool clientNeedsUpdate = await UpdateManager.CheckClientUpdateAsync();
                expectedHash = UpdateManager.ExpectedHash;

                if (clientNeedsUpdate)
                {
                    UpdateStatus(LanguageManager.Get("Splash.InstallingUpdates"));
                    await UpdateManager.ApplyClientUpdateAsync();
                }

                bool allFilesExist = true;
                foreach (var item in downloads)
                {
                    string destPath = GetDestinationPath(item);
                    if (!File.Exists(destPath) && !IsDirectoryExtracted(item))
                    {
                        allFilesExist = false;
                        break;
                    }
                }

                if (!allFilesExist)
                {
                    await DownloadRequiredFiles();
                    await ProcessDownloadedFiles();
                }

                try
                {
                    if (Directory.Exists(tempFolder))
                        Directory.Delete(tempFolder, true);
                }
                catch { }

                UpdateStatus(LanguageManager.Get("Splash.PleaseWait"));
                _targetProgress = 100;
                await Task.Delay(600);
                LoadingFinished?.Invoke();
            }
            catch (Exception ex)
            {
                UpdateStatus(string.Format(LanguageManager.Get("Splash.DownloadError"), ex.Message));
                MessageBox.Show(
                    string.Format(LanguageManager.Get("Splash.DownloadErrorMessage"), ex.Message),
                    LanguageManager.Get("Splash.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task DownloadRequiredFiles()
        {
            for (int i = 0; i < downloads.Length; i++)
            {
                var item = downloads[i];
                string destPath = GetDestinationPath(item);
                bool shouldSkip = false;

                if (!needsUpdate)
                {
                    if (!string.IsNullOrEmpty(item.DestRelativePath) &&
                        item.DestRelativePath.Contains("Nelian.runtime") &&
                        File.Exists(destPath))
                    {
                        shouldSkip = true;
                    }
                }

                if (shouldSkip || File.Exists(destPath) || IsDirectoryExtracted(item))
                {
                    int progress = (i + 1) * 100 / downloads.Length;
                    _targetProgress = progress;
                    continue;
                }

                string tempPath = Path.Combine(tempFolder, item.TempFileName);
                await DownloadFile(item.Url, tempPath);
                int newProgress = (i + 1) * 100 / downloads.Length;
                _targetProgress = newProgress;
            }
        }

        private async Task ProcessDownloadedFiles()
        {
            foreach (var item in downloads)
            {
                string tempPath = Path.Combine(tempFolder, item.TempFileName);
                if (!File.Exists(tempPath))
                    continue;

                string destPath = GetDestinationPath(item);

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));

                    if (File.Exists(destPath))
                        File.Delete(destPath);

                    File.Move(tempPath, destPath);

                    if (item.IsZip)
                    {
                        string extractDir = GetExtractPath(item);
                        if (!Directory.Exists(extractDir) || Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories).Length == 0)
                        {
                            await ExtractZipAsync(destPath, extractDir);
                        }
                        if (File.Exists(destPath))
                            File.Delete(destPath);
                    }
                }
                catch (Exception ex)
                {
                    UpdateStatus(string.Format(LanguageManager.Get("Splash.ProcessError"), item.TempFileName, ex.Message));
                }
            }
        }

        private string GetDestinationPath(DownloadItem item)
        {
            if (!string.IsNullOrEmpty(item.ProgramFilesDestRelativePath))
                return Path.Combine(ProgramFilesRoot, item.ProgramFilesDestRelativePath);
            else if (!string.IsNullOrEmpty(item.DestRelativePath))
            {
                if (item.DestRelativePath.Contains("Nelian.runtime"))
                    return Path.Combine(ProgramFilesRoot, item.DestRelativePath);
                return Path.Combine(appDataRoot, item.DestRelativePath);
            }
            else if (!string.IsNullOrEmpty(item.MinecraftDestRelativePath))
                return Path.Combine(MinecraftRoot, item.MinecraftDestRelativePath);
            else if (!string.IsNullOrEmpty(item.JavaDestRelativePath))
                return Path.Combine(JavaRoot, item.JavaDestRelativePath + ".zip");
            else
                return Path.Combine(tempFolder, item.TempFileName);
        }

        private string GetExtractPath(DownloadItem item)
        {
            if (!string.IsNullOrEmpty(item.JavaDestRelativePath))
                return Path.Combine(JavaRoot, item.JavaDestRelativePath);
            else
                return Path.Combine(appDataRoot, Path.GetFileNameWithoutExtension(item.DestRelativePath));
        }

        private bool IsDirectoryExtracted(DownloadItem item)
        {
            if (item.IsZip && !string.IsNullOrEmpty(item.JavaDestRelativePath))
            {
                string extractDir = Path.Combine(JavaRoot, item.JavaDestRelativePath);
                return Directory.Exists(extractDir) && Directory.GetFiles(extractDir, "*", SearchOption.AllDirectories).Length > 0;
            }
            return false;
        }

        private async Task DownloadFile(string url, string destPath)
        {
            using (WebClient client = new WebClient())
            {
                var tcs = new TaskCompletionSource<bool>();
                client.DownloadProgressChanged += (s, e) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        _targetProgress = e.ProgressPercentage;
                    }));
                };

                client.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error != null)
                        tcs.TrySetException(e.Error);
                    else
                        tcs.TrySetResult(true);
                };

                try
                {
                    client.DownloadFileAsync(new Uri(url), destPath);
                    await tcs.Task;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(LanguageManager.Get("Splash.DownloadFileFailed"), url, ex.Message));
                }
            }
        }

        private async Task ExtractZipAsync(string zipPath, string extractDir)
        {
            try
            {
                if (Directory.Exists(extractDir))
                {
                    try { Directory.Delete(extractDir, true); }
                    catch { }
                }

                Directory.CreateDirectory(extractDir);

                await Task.Run(() =>
                {
                    using (var archive = ZipFile.OpenRead(zipPath))
                    {
                        int total = archive.Entries.Count;
                        int current = 0;

                        foreach (var entry in archive.Entries)
                        {
                            if (string.IsNullOrEmpty(entry.Name))
                                continue;

                            string filePath = Path.Combine(extractDir, entry.FullName);
                            string fileDir = Path.GetDirectoryName(filePath);

                            if (!string.IsNullOrEmpty(fileDir))
                                Directory.CreateDirectory(fileDir);

                            entry.ExtractToFile(filePath, true);

                            current++;
                            int progress = (current * 100) / total;
                            BeginInvoke(new Action(() =>
                            {
                                _targetProgress = progress;
                            }));
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(LanguageManager.Get("Splash.ExtractFailed"), ex.Message));
            }
        }

        private void UpdateStatus(string message)
        {
            BeginInvoke(new Action(() =>
            {
                label1.Text = message;
            }));
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e) { }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.FindForm() != null)
            {
                ReleaseCapture();
                SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void Splash_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.FindForm() != null)
            {
                ReleaseCapture();
                SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.FindForm() != null)
            {
                ReleaseCapture();
                SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void guna2ProgressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.FindForm() != null)
            {
                ReleaseCapture();
                SendMessage(this.FindForm().Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void label1_TextChanged(object sender, EventArgs e) { }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UpdateManager.ProgressChanged -= OnUpdateProgressChanged;
                _smoothTimer?.Stop();
                _smoothTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void LoadingBar_ValueChanged(object sender, EventArgs e)
        {
            CenterLabel();
        }
    }
}

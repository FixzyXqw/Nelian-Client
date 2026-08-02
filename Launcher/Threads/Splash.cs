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
            public bool IsZip { get; set; }
        }

        private readonly DownloadItem[] downloads = new DownloadItem[]
        {
            new DownloadItem
            {
                Url = "https://github.com/FixzyXqw/Nelian/releases/download/NelianClientUpdate/Nelian.jar",
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
                Url = "https://github.com/FixzyXqw/Nelian/releases/download/Assets/jdk-8.0.492.9-hotspot.zip",
                TempFileName = "jdk-8.0.492.9-hotspot.tmp",
                JavaDestRelativePath = @"jdk-8.0.492.9-hotspot",
                IsZip = true
            }
        };

        private string appDataRoot;
        private string tempFolder;
        private string MinecraftRoot;
        private string JavaRoot;
        private string ProgramFilesRoot;
        public event Action LoadingFinished;

        private const string HASH_CHECK_URL = "https://raw.githubusercontent.com/FixzyXqw/Nelian/main/FAX.txt";
        private string expectedHash;
        private bool needsUpdate = false;

        private string configFilePath;

        public Splash()
        {

            InitializeComponent();


            guna2ProgressBar1.Minimum = 0;
            guna2ProgressBar1.Maximum = 100;
            guna2ProgressBar1.Value = 0;
            label1.Text = LanguageManager.Get("Splash.CheckingUpdates");
        }
        private void CenterControls()
        {
            CenterLabel();
            guna2ProgressBar1.Left = (this.ClientSize.Width - guna2ProgressBar1.Width) / 2;
            pictureBox1.Left = label1.Left + (label1.Width - pictureBox1.Width) / 2;
            pictureBox1.Top = label1.Top - pictureBox1.Height - 25;
        }
        private void CenterPictureBox()
        {
            BuildInfo.Tuvan("Code Name: MAYBETHATSNICE!");
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

                bool launcherNeedsUpdate = await CheckManifestDifference();

                if (launcherNeedsUpdate)
                {
                    UpdateStatus(LanguageManager.Get("Splash.InstallingUpdates"));

                    string updaterPath = Path.Combine(
                        ProgramFilesRoot,
                        "Updater.exe");

                    if (File.Exists(updaterPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = updaterPath,
                            UseShellExecute = true,
                            Verb = "runas"
                        });

                        Application.Exit();
                        return;
                    }

                    throw new FileNotFoundException(
                        "Updater.exe not found.",
                        updaterPath);
                }

                UpdateStatus(LanguageManager.Get("Splash.CheckingUpdates"));

                bool clientNeedsUpdate = await CheckClientUpdate();

                if (clientNeedsUpdate)
                {
                    UpdateStatus(LanguageManager.Get("Splash.InstallingUpdates"));

                    var clientItem = downloads.First(x =>
                        !string.IsNullOrEmpty(x.DestRelativePath) &&
                        x.DestRelativePath.Equals(
                            "Nelian.runtime",
                            StringComparison.OrdinalIgnoreCase));

                    string tempPath = Path.Combine(
                        tempFolder,
                        clientItem.TempFileName);

                    await DownloadFile(
                        clientItem.Url,
                        tempPath);

                    string destPath = GetDestinationPath(clientItem);

                    Directory.CreateDirectory(
                        Path.GetDirectoryName(destPath));

                    if (File.Exists(destPath))
                        File.Delete(destPath);

                    File.Move(
                        tempPath,
                        destPath);

                    await VerifyInstalledHash();
                }

                bool allFilesExist = true;

                foreach (var item in downloads)
                {
                    string destPath = GetDestinationPath(item);

                    if (!File.Exists(destPath) &&
                        !IsDirectoryExtracted(item))
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
                catch
                {
                }

                UpdateStatus(LanguageManager.Get("Splash.PleaseWait"));

                UpdateProgress(100);

                await Task.Delay(600);

                LoadingFinished?.Invoke();
            }
            catch (Exception ex)
            {
                UpdateStatus(
                    string.Format(
                        LanguageManager.Get("Splash.DownloadError"),
                        ex.Message));

                MessageBox.Show(
                    string.Format(
                        LanguageManager.Get("Splash.DownloadErrorMessage"),
                        ex.Message),
                    LanguageManager.Get("Splash.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private async Task<bool> CheckClientUpdate()
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                string remoteHash = await client.GetStringAsync(HASH_CHECK_URL);

                remoteHash = remoteHash.Trim();

                if (string.IsNullOrWhiteSpace(remoteHash))
                    throw new Exception("FAX.txt boş.");

                expectedHash = remoteHash;

                string localPath = Path.Combine(
                    ProgramFilesRoot,
                    "Nelian.runtime");

                if (!File.Exists(localPath))
                    return true;

                string localHash = await CalculateFileHashAsync(localPath);

                return !string.Equals(
                    localHash,
                    expectedHash,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return true;
            }
        }
        private async Task CheckForUpdates()
        {
            try
            {
                needsUpdate = await CheckManifestDifference();

                if (needsUpdate)
                    UpdateStatus(LanguageManager.Get("Splash.InstallingUpdates"));
                else
                    UpdateStatus(LanguageManager.Get("Splash.AllSet"));
            }
            catch (Exception ex)
            {
                needsUpdate = true;
                UpdateStatus(string.Format(LanguageManager.Get("Splash.UpdateCheckFailed"), ex.Message));
            }
        }

        private const string MANIFEST_URL = "https://raw.githubusercontent.com/FixzyXqw/Nelian/refs/heads/main/manifest.json";

        private class Manifest
        {
            public string version { get; set; }
            public System.Collections.Generic.List<FileEntry> files { get; set; }
        }

        private class FileEntry
        {
            public string path { get; set; }
            public string sha1 { get; set; }
        }

        private async Task<bool> CheckManifestDifference()
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                string remoteManifestJson = await client.GetStringAsync(MANIFEST_URL);
                var remoteManifest = JsonSerializer.Deserialize<Manifest>(remoteManifestJson);

                string localManifestPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Nelian",
                    "manifest.json");

                if (!File.Exists(localManifestPath))
                    return true;

                string localManifestJson = await File.ReadAllTextAsync(localManifestPath);
                var localManifest = JsonSerializer.Deserialize<Manifest>(localManifestJson);

                if (remoteManifest.version != localManifest.version)
                    return true;

                var remoteFiles = remoteManifest.files
                    .Where(x => !RuntimeFilter.IsDotNetRuntimeFile(x.path))
                    .ToDictionary(x => x.path, x => x.sha1);

                var localFiles = localManifest.files
                    .Where(x => !RuntimeFilter.IsDotNetRuntimeFile(x.path))
                    .ToDictionary(x => x.path, x => x.sha1);

                if (remoteFiles.Count != localFiles.Count)
                    return true;

                foreach (var file in remoteFiles)
                {
                    if (!localFiles.TryGetValue(file.Key, out string sha1))
                        return true;

                    if (!string.Equals(file.Value, sha1, StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        private async Task<string> CalculateFileHashAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                using (var sha = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            });
        }

        private async Task VerifyInstalledHash()
        {
            try
            {
                string nelianRuntimePath = Path.Combine(ProgramFilesRoot, "Nelian.runtime");

                if (!File.Exists(nelianRuntimePath))
                {
                    throw new Exception(LanguageManager.Get("Splash.HashMismatch"));
                }

                string installedHash = await CalculateFileHashAsync(nelianRuntimePath);

                if (!string.Equals(installedHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(LanguageManager.Get("Splash.HashVerificationFailed"));
                }

                UpdateStatus(LanguageManager.Get("Splash.PleaseWait"));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(LanguageManager.Get("Splash.HashError"), ex.Message));
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
                    UpdateProgress(progress);
                    continue;
                }

                string tempPath = Path.Combine(tempFolder, item.TempFileName);

                await DownloadFile(item.Url, tempPath);

                int newProgress = (i + 1) * 100 / downloads.Length;
                UpdateProgress(newProgress);
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
            if (!string.IsNullOrEmpty(item.DestRelativePath))
            {
                if (item.DestRelativePath.Contains("Nelian.runtime"))
                {
                    return Path.Combine(ProgramFilesRoot, item.DestRelativePath);
                }
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
                        guna2ProgressBar1.Value = e.ProgressPercentage;
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
                        foreach (var entry in archive.Entries)
                        {
                            if (string.IsNullOrEmpty(entry.Name))
                                continue;

                            string filePath = Path.Combine(extractDir, entry.FullName);
                            string fileDir = Path.GetDirectoryName(filePath);

                            if (!string.IsNullOrEmpty(fileDir))
                                Directory.CreateDirectory(fileDir);

                            entry.ExtractToFile(filePath, true);
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

        private void UpdateProgress(int value)
        {
            BeginInvoke(new Action(() =>
            {
                guna2ProgressBar1.Value = value;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            guna2ProgressBar1.Text = label1.Text;
        }


        private void guna2ProgressBar1_ValueChanged_1(object sender, EventArgs e)
        {
            CenterLabel();
        }
    }
}

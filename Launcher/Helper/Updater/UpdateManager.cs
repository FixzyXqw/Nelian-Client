using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nelian
{
    public static class UpdateManager
    {
        private static string _expectedHash;
        public static string ExpectedHash => _expectedHash;

        private const string MANIFEST_URL = "https://raw.githubusercontent.com/FixzyXqw/Nelian-Client/main/manifest.json";
        private const string HASH_CHECK_URL = "https://raw.githubusercontent.com/FixzyXqw/Nelian-Client/main/FAX.Update";
        private const string CLIENT_DOWNLOAD_URL = "https://github.com/FixzyXqw/Nelian-Client/releases/download/NelianClientUpdate/Nelian.jar";
        private const string GUARD_DOWNLOAD_URL = "https://github.com/FixzyXqw/Nelian-Client/releases/download/NelianClientUpdate/NelianGuard.dll";
        private const string GUARD_HASH_CHECK_URL = "https://raw.githubusercontent.com/FixzyXqw/Nelian-Client/main/NelianGuard.hash";

        private static string ProgramFilesRoot => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            "Nelian");

        private static string LocalManifestPath => Path.Combine(ProgramFilesRoot, "manifest.json");
        private static string LocalClientPath => Path.Combine(ProgramFilesRoot, "Nelian.runtime");
        private static string LocalGuardPath => Path.Combine(ProgramFilesRoot, "NelianGuard.dll");
        private static string UpdaterPath => Path.Combine(ProgramFilesRoot, "Updater.exe");

        public static event Action<int>? ProgressChanged;

        private static void ReportProgress(int value)
        {
            ProgressChanged?.Invoke(value);
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            return client;
        }

        private static string GetCacheBustedUrl(string baseUrl)
        {
            string separator = baseUrl.Contains("?") ? "&" : "?";
            return $"{baseUrl}{separator}_={DateTime.UtcNow.Ticks}";
        }

        public static async Task<bool> CheckLauncherUpdateAsync()
        {
            try
            {
                using var client = CreateHttpClient();
                string url = GetCacheBustedUrl(MANIFEST_URL);
                string remoteManifestJson = await client.GetStringAsync(url);
                var remoteManifest = JsonSerializer.Deserialize<Manifest>(remoteManifestJson);

                if (remoteManifest == null)
                    return false;

                if (!File.Exists(LocalManifestPath))
                    return true;

                string localManifestJson = await File.ReadAllTextAsync(LocalManifestPath);
                var localManifest = JsonSerializer.Deserialize<Manifest>(localManifestJson);

                if (localManifest == null)
                    return true;

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
                return false;
            }
        }

        public static async Task<bool> CheckClientUpdateAsync()
        {
            try
            {
                using var client = CreateHttpClient();
                string url = GetCacheBustedUrl(HASH_CHECK_URL);
                string remoteHash = await client.GetStringAsync(url);
                remoteHash = remoteHash.Trim();

                if (string.IsNullOrWhiteSpace(remoteHash))
                    return false;

                _expectedHash = remoteHash;

                if (!File.Exists(LocalClientPath))
                    return true;

                string localHash = await CalculateFileHashAsync(LocalClientPath);
                return !string.Equals(localHash, _expectedHash, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CheckGuardUpdateAsync()
        {
            try
            {
                using var client = CreateHttpClient();
                string url = GetCacheBustedUrl(GUARD_HASH_CHECK_URL);
                string remoteHash = await client.GetStringAsync(url);
                remoteHash = remoteHash.Trim();

                if (string.IsNullOrWhiteSpace(remoteHash))
                    return false;

                if (!File.Exists(LocalGuardPath))
                    return true;

                string localHash = await CalculateFileHashAsync(LocalGuardPath);
                return !string.Equals(localHash, remoteHash, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return !File.Exists(LocalGuardPath);
            }
        }

        public static async Task<bool> ApplyGuardUpdateAsync()
        {
            try
            {
                ReportProgress(10);

                string tempFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Game", "Nelian", "temp");
                Directory.CreateDirectory(tempFolder);
                string tempPath = Path.Combine(tempFolder, "NelianGuard.tmp");

                ReportProgress(20);

                await DownloadFileWithProgressAsync(GUARD_DOWNLOAD_URL, tempPath);

                ReportProgress(70);

                if (File.Exists(LocalGuardPath))
                {
                    string backupPath = LocalGuardPath + ".old";
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Move(LocalGuardPath, backupPath);
                }

                File.Move(tempPath, LocalGuardPath);

                ReportProgress(85);

                string installedHash = await CalculateFileHashAsync(LocalGuardPath);

                using var client = CreateHttpClient();
                string url = GetCacheBustedUrl(GUARD_HASH_CHECK_URL);
                string remoteHash = await client.GetStringAsync(url);
                remoteHash = remoteHash.Trim();

                if (!string.IsNullOrWhiteSpace(remoteHash) &&
                    !string.Equals(installedHash, remoteHash, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "NelianGuard.dll hash verification failed!",
                        "Update Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ReportProgress(0);
                    return false;
                }

                ReportProgress(100);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Failed to update NelianGuard.dll: {0}", ex.Message),
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ReportProgress(0);
                return false;
            }
        }

        public static async Task<UpdateCheckResult> CheckAllUpdatesAsync()
        {
            bool launcher = await CheckLauncherUpdateAsync();
            if (launcher)
                return UpdateCheckResult.LauncherUpdate;

            bool client = await CheckClientUpdateAsync();
            if (client)
                return UpdateCheckResult.ClientUpdate;

            bool guard = await CheckGuardUpdateAsync();
            if (guard)
                return UpdateCheckResult.GuardUpdate;

            return UpdateCheckResult.NoUpdate;
        }

        public static void ApplyLauncherUpdate()
        {
            if (!File.Exists(UpdaterPath))
            {
                MessageBox.Show(
                    LanguageManager.Get("Splash.UpdaterNotFound"),
                    LanguageManager.Get("Splash.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = UpdaterPath,
                UseShellExecute = true,
                Verb = "runas"
            });

            Application.Exit();
        }

        public static async Task<bool> ApplyClientUpdateAsync()
        {
            try
            {
                ReportProgress(10);

                string tempFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Microsoft", "Game", "Nelian", "temp");
                Directory.CreateDirectory(tempFolder);
                string tempPath = Path.Combine(tempFolder, "Secure.tmp");

                ReportProgress(20);

                await DownloadFileWithProgressAsync(CLIENT_DOWNLOAD_URL, tempPath);

                ReportProgress(70);

                if (File.Exists(LocalClientPath))
                    File.Delete(LocalClientPath);
                File.Move(tempPath, LocalClientPath);

                ReportProgress(85);

                string installedHash = await CalculateFileHashAsync(LocalClientPath);
                if (!string.Equals(installedHash, _expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        LanguageManager.Get("Splash.HashVerificationFailed"),
                        LanguageManager.Get("Splash.Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    ReportProgress(0);
                    return false;
                }

                ReportProgress(100);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(LanguageManager.Get("Splash.DownloadError"), ex.Message),
                    LanguageManager.Get("Splash.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                ReportProgress(0);
                return false;
            }
        }

        private static async Task DownloadFileWithProgressAsync(string url, string destPath)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var canReportProgress = totalBytes > 0;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None);

            var buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalRead += bytesRead;

                if (canReportProgress)
                {
                    int progress = (int)((totalRead * 100) / totalBytes);
                    progress = Math.Min(100, Math.Max(20, progress));
                    ReportProgress(20 + (progress * 50 / 100));
                }
            }
        }

        private static async Task<string> CalculateFileHashAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                using var sha = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            });
        }

        private static async Task DownloadFileAsync(string url, string destPath)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            using var fs = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }

        private class Manifest
        {
            public string version { get; set; }
            public List<FileEntry> files { get; set; }
        }

        private class FileEntry
        {
            public string path { get; set; }
            public string sha1 { get; set; }
        }
    }

    public enum UpdateCheckResult
    {
        NoUpdate,
        LauncherUpdate,
        ClientUpdate,
        GuardUpdate
    }
}

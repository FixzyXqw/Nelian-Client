using CmlLib.Core.Installer.Forge;
using CmlLib.Core.ModLoaders.QuiltMC;
using Nelian.Managers;
using Nelian.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Nelian.Installer
{
    public class MrPackInstaller
    {
        public static bool ReadIndex(string mrpackPath, out ModrinthIndex index, out string error)
        {
            index = null;
            error = null;
            try
            {
                using (var archive = ZipFile.OpenRead(mrpackPath))
                {
                    var entry = archive.GetEntry("modrinth.index.json");
                    if (entry == null)
                    {
                        error = "modrinth.index.json bulunamadı!";
                        return false;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var json = reader.ReadToEnd();
                        index = JsonSerializer.Deserialize<ModrinthIndex>(json);
                        if (index == null)
                        {
                            error = "modrinth.index.json okunamadı!";
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = $"Hata: {ex.Message}";
                return false;
            }
        }
        public static async Task<bool> InstallAsync(string mrpackPath, Action<string, int, int> onProgress = null)
        {
            if (!File.Exists(mrpackPath))
            {
                MessageBox.Show("Dosya bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            onProgress?.Invoke("Paket okunuyor...", 0, 100);
            if (!ReadIndex(mrpackPath, out var index, out var error))
            {
                MessageBox.Show($"Paket okunamadı:\n{error}", "Import Hatası",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string name = index.Name ?? index.VersionId ?? "Unknown Pack";
            string mcVersion = index.Dependencies?.Minecraft ?? "Unknown";
            string loader = "unknown";
            string loaderVersion = "unknown";
            if (!string.IsNullOrEmpty(index.Dependencies?.FabricLoader))
            {
                loader = "fabric";
                loaderVersion = index.Dependencies.FabricLoader;
            }
            else if (!string.IsNullOrEmpty(index.Dependencies?.NeoForge))
            {
                loader = "neoforge";
                loaderVersion = index.Dependencies.NeoForge;
            }
            else if (!string.IsNullOrEmpty(index.Dependencies?.Forge))
            {
                loader = "forge";
                loaderVersion = index.Dependencies.Forge;
            }
            else if (!string.IsNullOrEmpty(index.Dependencies?.QuiltLoader))
            {
                loader = "quilt";
                loaderVersion = index.Dependencies.QuiltLoader;
            }
            string loaderDisplay = loader;
            if (loaderDisplay == "fabric") loaderDisplay = "🧵 Fabric";
            else if (loaderDisplay == "forge") loaderDisplay = "🔨 Forge";
            else if (loaderDisplay == "quilt") loaderDisplay = "🧶 Quilt";
            else loaderDisplay = loader;
            try
            {
                onProgress?.Invoke("...", 30, 100);
                var instance = InstanceManager.Create(name, mcVersion, loader, loaderVersion);
                string versionId = mcVersion;
                switch (loader)
                {
                    case "fabric":
                        versionId = await FabricInstaller.InstallAsync(
                            instance.Path,
                            mcVersion,
                            loaderVersion);
                        break;
                    case "forge":
                        versionId = await ForgeInstaller.InstallAsync(
                            instance.Path,
                            mcVersion,
                            loaderVersion);
                        break;
                    case "neoforge":
                        versionId = await NeoForgeInstaller.InstallAsync(
                            instance.Path,
                            mcVersion,
                            loaderVersion);
                        break;
                    case "quilt":
                        break;
                }
                instance.VersionId = versionId;
                InstanceManager.SaveInstance(instance);
                if (index.Files != null && index.Files.Count > 0)
                {
                    onProgress?.Invoke($"0/{index.Files.Count} dosya indiriliyor...", 10, 100);
                    using (var client = new WebClient())
                    {
                        int downloaded = 0;
                        int total = index.Files.Count;
                        foreach (var file in index.Files)
                        {
                            if (file.Downloads == null || file.Downloads.Count == 0)
                                continue;
                            var targetPath = Path.Combine(instance.Path, file.Path);
                            var targetDir = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                                Directory.CreateDirectory(targetDir);
                            string url = file.Downloads[0];
                            await client.DownloadFileTaskAsync(new Uri(url), targetPath);
                            downloaded++;
                            int progress = 10 + (int)((double)downloaded / total * 70);
                            onProgress?.Invoke($"{downloaded}/{total} dosya indirildi", progress, 100);
                        }
                    }
                }
                onProgress?.Invoke("Overrides kopyalanıyor...", 85, 100);
                CopyOverrides(mrpackPath, instance.Path);
                onProgress?.Invoke("Tamamlandı!", 100, 100);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Instance oluşturulurken hata:\n{ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        private static void CopyOverrides(string mrpackPath, string instancePath)
        {
            try
            {
                using (var archive = ZipFile.OpenRead(mrpackPath))
                {
                    int copiedCount = 0;
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.StartsWith("overrides/") && !entry.FullName.EndsWith("/"))
                        {
                            var relativePath = entry.FullName.Substring("overrides/".Length);
                            var targetPath = Path.Combine(instancePath, relativePath);
                            var targetDir = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir))
                                Directory.CreateDirectory(targetDir);
                            entry.ExtractToFile(targetPath, true);
                            copiedCount++;
                        }
                    }
                    if (copiedCount > 0)
                        Console.WriteLine($"{copiedCount} dosya overrides'dan kopyalandı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Overrides dosyaları kopyalanırken hata:\n{ex.Message}",
                    "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private static string CalculateTotalSize(List<ModrinthFile> files)
        {
            if (files == null || files.Count == 0)
                return "0 MB";
            long totalBytes = 0;
            foreach (var file in files)
                totalBytes += file.FileSize;
            double mb = totalBytes / (1024.0 * 1024.0);
            if (mb < 1)
                return $"{totalBytes / 1024.0:F1} KB";
            else if (mb < 1024)
                return $"{mb:F1} MB";
            else
                return $"{mb / 1024.0:F1} GB";
        }
    }
}

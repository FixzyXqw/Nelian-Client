using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nelian.Installer
{
    public static class FabricInstaller
    {
        public static async Task<string> InstallAsync(string minecraftFolder, string mcVersion, string loaderVersion)
        {
            using var client = new HttpClient();

            string profileUrl =
                $"https://meta.fabricmc.net/v2/versions/loader/{mcVersion}/{loaderVersion}/profile/json";

            string json = await client.GetStringAsync(profileUrl);

            using var doc = JsonDocument.Parse(json);

            string id = doc.RootElement.GetProperty("id").GetString();

            string versionsDir = Path.Combine(minecraftFolder, "versions", id);
            Directory.CreateDirectory(versionsDir);

            File.WriteAllText(
                Path.Combine(versionsDir, id + ".json"),
                json);

            return id;
        }
    }
}

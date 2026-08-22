// Models/ModrinthIndex.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Nelian.Models
{
    public class ModrinthIndex
    {
        [JsonPropertyName("formatVersion")]
        public int FormatVersion { get; set; }

        [JsonPropertyName("game")]
        public string Game { get; set; }

        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("dependencies")]
        public ModrinthDependencies Dependencies { get; set; }

        [JsonPropertyName("files")]
        public List<ModrinthFile> Files { get; set; }
    }

    public class ModrinthDependencies
    {
        [JsonPropertyName("minecraft")]
        public string Minecraft { get; set; }

        [JsonPropertyName("fabric-loader")]
        public string FabricLoader { get; set; }

        [JsonPropertyName("forge")]
        public string Forge { get; set; }

        [JsonPropertyName("neoforge")]
        public string NeoForge { get; set; }

        [JsonPropertyName("quilt-loader")]
        public string QuiltLoader { get; set; }
    }

    public class ModrinthFile
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("hashes")]
        public ModrinthHashes Hashes { get; set; }

        [JsonPropertyName("downloads")]
        public List<string> Downloads { get; set; }

        [JsonPropertyName("fileSize")]
        public int FileSize { get; set; }

        [JsonPropertyName("env")]
        public ModrinthEnv Env { get; set; }
    }

    public class ModrinthHashes
    {
        [JsonPropertyName("sha512")]
        public string Sha512 { get; set; }

        [JsonPropertyName("sha1")]
        public string Sha1 { get; set; }
    }

    public class ModrinthEnv
    {
        [JsonPropertyName("client")]
        public string Client { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }
    }
}

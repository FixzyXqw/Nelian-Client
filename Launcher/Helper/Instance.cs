using System;

namespace Nelian.Models
{
    public class Instance
    {
        public string Name { get; set; }
        public string MinecraftVersion { get; set; }
        public string Loader { get; set; }
        public string LoaderVersion { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPlayed { get; set; }
        public string VersionId { get; set; }
        public Instance()
        {
            CreatedAt = DateTime.Now;
            LastPlayed = DateTime.Now;
        }
    }
}

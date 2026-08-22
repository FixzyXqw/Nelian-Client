using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Nelian.Models;
namespace Nelian.Managers
{
    public class InstanceManager
    {
        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft", "Game", "Nelian"
        );
        private static readonly string InstancesPath = Path.Combine(AppDataPath, "Instances");
        private static readonly string InstanceConfigFile = "instance.json";
        static InstanceManager()
        {
            if (!Directory.Exists(InstancesPath))
                Directory.CreateDirectory(InstancesPath);
        }
        public static string GetInstancesPath() => InstancesPath;
        public static Instance Create(string name, string mcVersion, string loader, string loaderVersion)
        {
            var instance = new Instance
            {
                Name = name,
                MinecraftVersion = mcVersion,
                Loader = loader,
                LoaderVersion = loaderVersion,
                Path = Path.Combine(InstancesPath, SanitizeFolderName(name))
            };
            if (!Directory.Exists(instance.Path))
                Directory.CreateDirectory(instance.Path);
            SaveInstance(instance);
            return instance;
        }
        public static void Delete(string instanceName)
        {
            var instance = Get(instanceName);
            if (instance == null) return;
            if (Directory.Exists(instance.Path))
                Directory.Delete(instance.Path, true);
        }
        public static Instance Get(string instanceName)
        {
            var instances = GetAll();
            return instances.FirstOrDefault(i => i.Name == instanceName);
        }
        public static List<Instance> GetAll()
        {
            var instances = new List<Instance>();
            if (!Directory.Exists(InstancesPath))
                return instances;
            foreach (var dir in Directory.GetDirectories(InstancesPath))
            {
                var configPath = Path.Combine(dir, InstanceConfigFile);
                if (File.Exists(configPath))
                {
                    try
                    {
                        var json = File.ReadAllText(configPath);
                        var instance = JsonSerializer.Deserialize<Instance>(json);
                        if (instance != null)
                            instances.Add(instance);
                    }
                    catch {  }
                }
            }
            return instances;
        }
        public static void SaveInstance(Instance instance)
        {
            var configPath = Path.Combine(instance.Path, InstanceConfigFile);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(instance, options);
            File.WriteAllText(configPath, json);
        }
        public static void UpdateInstance(Instance instance)
        {
            SaveInstance(instance);
        }
        private static string SanitizeFolderName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());
            return string.IsNullOrEmpty(sanitized) ? "Instance" : sanitized;
        }
    }
}

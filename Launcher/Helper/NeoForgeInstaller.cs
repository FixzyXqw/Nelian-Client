using CmlLib.Core;
using CmlLib.Core.Installer.NeoForge;
using System.Threading.Tasks;

namespace Nelian.Installer
{
    public static class NeoForgeInstaller
    {
        public static async Task<string> InstallAsync(
            string minecraftFolder,
            string mcVersion,
            string neoForgeVersion)
        {
            var launcher = new MinecraftLauncher(
                new MinecraftPath(minecraftFolder));

            var installer = new CmlLib.Core.Installer.NeoForge.NeoForgeInstaller(
                launcher);

            var version = await installer.Install(
                mcVersion,
                neoForgeVersion);

            return version;
        }
    }
}

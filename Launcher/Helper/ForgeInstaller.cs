using CmlForgeInstaller = CmlLib.Core.Installer.Forge.ForgeInstaller;
using CmlLib.Core;
using System.Threading.Tasks;

namespace Nelian.Installer
{
    public static class ForgeInstaller
    {
        public static async Task<string> InstallAsync(
            string minecraftFolder,
            string mcVersion,
            string forgeVersion)
        {
            var launcher = new MinecraftLauncher(
                new MinecraftPath(minecraftFolder));

            var installer = new CmlForgeInstaller(
                launcher);

            var version = await installer.Install(
                mcVersion,
                forgeVersion);

            return version;
        }
    }
}

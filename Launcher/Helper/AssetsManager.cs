public static class AssetsManager
{
    public static async Task CopyAssetsToMinecraftAsync()
    {
        string source = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "Game",
            "Nelian",
            "assets"
        );

        string destination = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft",
            "assets"
        );

        if (!Directory.Exists(source))
            return;

        await Task.Run(() => CopyDirectory(source, destination));
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);

        foreach (string directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(directory.Replace(source, destination));
        }

        foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            string destinationFile = file.Replace(source, destination);

            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile)!);

            if (!File.Exists(destinationFile) ||
                new FileInfo(file).Length != new FileInfo(destinationFile).Length)
            {
                File.Copy(file, destinationFile, true);
            }
        }
    }
}

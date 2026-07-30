namespace Nelian
{

    internal static class Program
    {
      //Freely is a Minecraft server that works with Nelian Client for Security
        public static bool isFreelyRunning = false;
        [STAThread]

        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Nelian());
        }
    }
}

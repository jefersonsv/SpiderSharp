using System.Runtime.InteropServices;

namespace ScrapShell
{
    public class Program
    {
        private static void Main(string[] args)
        {
            bool hasRegistry = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (hasRegistry)
            {
                ChromeLauncher.OpenLink(@"file:///C:/Users/Administrator/AppData/Local/Temp/2/tmp6344.tmp.html");
            }
            else
            {
                System.Console.WriteLine("Platform not supported");
            }
        }
    }
}
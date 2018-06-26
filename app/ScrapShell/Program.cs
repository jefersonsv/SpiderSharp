using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ScrapShell
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || !args.Any())
            {
                // https://github.com/tonerdo/readline
                Console.WriteLine("Enter url: ");
                string input = ReadLine.Read("# ");

                
            }
            

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
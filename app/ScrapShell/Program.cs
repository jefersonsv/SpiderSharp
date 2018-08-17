using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ScrapShell
{
    public class Program
    {
        static readonly Regex loadRegex = new Regex("load (?<url>.*)");
        static readonly Regex saveRegex = new Regex("save (?<filename>.*)");
        static readonly Regex browseRegex = new Regex("browse");
        static readonly Regex quitRegex = new Regex("quit");
        static readonly Regex helpRegex = new Regex("help");

        private static void Main(string[] args)
        {
            if (args == null || !args.Any())
            {
                ReadLine.HistoryEnabled = true;

                // https://github.com/tonerdo/readline

                var content = string.Empty;

                Console.Clear();
                PrintCommands();
                while (true)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine("Type command: ");
                    string input = ReadLine.Read("# ");

                    if (loadRegex.IsMatch(input))
                    {
                        var url = loadRegex.Match(input).Groups["url"].Value;

                        Uri uri = null;
                        if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                        {
                            if (File.Exists(url))
                                content = System.IO.File.ReadAllText(url);
                            else
                                Console.WriteLine("file not found");
                        }
                        else
                        {
                            HttpRequester.ChromeHeadlessPersistentClient httpRequester = new HttpRequester.ChromeHeadlessPersistentClient();
                            content = httpRequester.GoAndGetContentAsync(url).Result;
                        }

                        Console.WriteLine("Loaded");
                    }

                    if (saveRegex.IsMatch(input))
                    {
                        var filename = saveRegex.Match(input).Groups["filename"].Value;
                        if (string.IsNullOrEmpty(content))
                        {
                            Console.WriteLine("You must load some url before execute save command");
                            continue;
                        }
                        
                        System.IO.File.WriteAllText(filename, content);

                        Console.WriteLine("Saved");
                    }

                    if (browseRegex.IsMatch(input))
                    {
                        if (string.IsNullOrEmpty(content))
                        {
                            Console.WriteLine("You must load some content before execute open command");
                            continue;
                        }
                        
                        var tmp = System.IO.Path.GetTempFileName() + ".html";
                        System.IO.File.WriteAllText(tmp, content);
                        OpenBrowser(tmp);

                        Console.WriteLine("Browser opened");
                    }

                    if (helpRegex.IsMatch(input))
                    {
                        PrintCommands();
                    }

                    if (quitRegex.IsMatch(input))
                    {
                        Console.WriteLine("Exiting");
                        break;
                    }
                }
            }
        }

        static void PrintCommands()
        {
            Console.WriteLine("load => load url or local file");
            Console.WriteLine("save => save content to local file");
            Console.WriteLine("browse => open browser with content");
            Console.WriteLine("quit => exit program");
            Console.WriteLine("help => show all commands");
        }

        static void OpenBrowser(string filename)
        {
            bool hasRegistry = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (hasRegistry)
            {
                ChromeLauncher.OpenFile(filename);
            }
            else
            {
                System.Console.WriteLine("Platform not supported");
            }
        }
    }
}

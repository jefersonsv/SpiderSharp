using ConsoleMenu;
using SpiderSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ScrapShell
{
    public class Program
    {
        static readonly Regex loadRegex = new Regex("load (?<url>.*)");
        static readonly Regex setRegex = new Regex("set (?<driver>.*)");
        static readonly Regex saveRegex = new Regex("save (?<filename>.*)");
        static readonly Regex cssRegex = new Regex("css (?<selector>.*)");
        static readonly Regex browseRegex = new Regex("browse");
        static readonly Regex quitRegex = new Regex("quit");
        static readonly Regex helpRegex = new Regex("help");
        static HttpRequester.Requester requester;

        private static void Main(string[] args)
        {
            //ReadLine.HistoryEnabled = true;

            //DrawMainMenu();
            //return;


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
                            if (requester != null)
                            {
                                content = requester.GetContentAsync(url).Result;
                            }
                            else
                            {
                                HttpRequester.ChromeHeadlessPersistentClient httpRequester = new HttpRequester.ChromeHeadlessPersistentClient();
                                content = httpRequester.GoAndGetContentAsync(url).Result;
                            }
                        }

                        Console.WriteLine($"{content.Count()} bytes loaded");
                    }

                    if (setRegex.IsMatch(input))
                    {
                        var driver = setRegex.Match(input).Groups["driver"].Value;
                        if (string.IsNullOrEmpty(driver))
                        {
                            Console.WriteLine("You must type on valid driver");
                            continue;
                        }

                        switch (driver.ToLower().Trim())
                        {
                            case "anglesharp":
                                requester = new HttpRequester.Requester(HttpRequester.EnumHttpProvider.AngleSharp);
                                Console.WriteLine("AngleSharp has been set");
                                break;
                        }
                    }

                    if (cssRegex.IsMatch(input))
                    {
                        var selector = setRegex.Match(input).Groups["selector"].Value;
                        if (string.IsNullOrEmpty(selector))
                        {
                            Console.WriteLine("You must type the selector");
                            continue;
                        }

                        Nodes nodes = new Nodes(content);
                        var res = nodes.SelectNodes(selector);
                        res.ToList().ForEach(a => Console.WriteLine(a));
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

        private static void DrawMainMenu()
        {
            var menu = string.Empty;

            do
            {
                Console.Clear();
                menu = new Menu().Render(new string[] { "Set Engine", "Load Page", "Save to disk", "Browse", "Help", "Quit" });
                switch (menu)
                {
                    case "Set Engine":
                        DrawSetEnginer();
                        break;

                    case "Load Page":
                        DrawLoadPage();
                        break;
                }
            } while (menu != "Quit");
        }

        private static void DrawSetEnginer()
        {
            Console.Clear();
            var menu = new Menu().Render(new string[] { "Set AngleSharp", "Set WebClient", "Back" });
            switch (menu)
            {
                case "Set AngleSharp":
                    requester = new HttpRequester.Requester(HttpRequester.EnumHttpProvider.AngleSharp);
                    return;
                case "Set WebClient":
                    requester = new HttpRequester.Requester(HttpRequester.EnumHttpProvider.WebClient);
                    return;
            }
        }

        private static void DrawLoadPage()
        {
            Console.Clear();
            Console.Write("Enter url: ");
            var url = ReadLine.Read("# ");

            var content = requester.GetContentAsync(url).Result;
        }

        static void PrintCommands()
        {
            Console.WriteLine("load => load url or local file");
            Console.WriteLine("save => save content to local file");
            Console.WriteLine("set => set HttpRequest that you want");
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

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
        static readonly Regex innertext = new Regex("innertext (?<selector>.*)");
        static readonly Regex innerhtml = new Regex("innerhtml (?<selector>.*)");
        static readonly Regex links = new Regex("links (?<selector>.*)");
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
                    string input = ReadLine.Read("# ");
                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    Console.WriteLine(string.Empty);
                    // Console.WriteLine("Type command: ");

                    if (loadRegex.IsMatch(input))
                    {
                        var url = loadRegex.Match(input).Groups["url"].Value;

                        if (File.Exists(url))
                        {
                            content = System.IO.File.ReadAllText(url);
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

                            var tmp = System.IO.Path.GetTempFileName() + ".html";
                            System.IO.File.WriteAllText(tmp, content);
                            Console.WriteLine($"Content saved: {tmp}");
                        }

                        Console.WriteLine($"{content.Count()} bytes loaded");
                        continue;
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

                        continue;
                    }

                    if (innertext.IsMatch(input))
                    {
                        var selector = innertext.Match(input).Groups["selector"].Value;
                        if (string.IsNullOrEmpty(selector))
                        {
                            Console.WriteLine("You must type the selector");
                            continue;
                        }

                        Nodes nodes = new Nodes(content);
                        var res = nodes.SelectNodes(selector);
                        res.ToList().ForEach(a => Console.WriteLine(a.GetInnerText()));

                        continue;
                    }

                    if (innerhtml.IsMatch(input))
                    {
                        var selector = innerhtml.Match(input).Groups["selector"].Value;
                        if (string.IsNullOrEmpty(selector))
                        {
                            Console.WriteLine("You must type the selector");
                            continue;
                        }

                        Nodes nodes = new Nodes(content);
                        var res = nodes.SelectNodes(selector);
                        res.ToList().ForEach(a => Console.WriteLine(a.GetInnerHtml()));

                        continue;
                    }

                    if (links.IsMatch(input))
                    {
                        var selector = links.Match(input).Groups["selector"].Value;
                        if (string.IsNullOrEmpty(selector))
                        {
                            Console.WriteLine("You must type the selector");
                            continue;
                        }

                        Nodes nodes = new Nodes(content);
                        var res = nodes.SelectHref(selector);
                        res.ToList().ForEach(a => Console.WriteLine(a));

                        continue;
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
                        continue;
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
                        continue;
                    }

                    if (helpRegex.IsMatch(input))
                    {
                        PrintCommands();
                        continue;
                    }

                    if (quitRegex.IsMatch(input))
                    {
                        Console.WriteLine("Exiting");
                        break;
                    }

                    Console.WriteLine("Command not found");
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
            Console.WriteLine("innertext => select innertext using css selector");
            Console.WriteLine("innerhtml => select innerhtml using css selector");
            Console.WriteLine("links => select links using css selector");
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

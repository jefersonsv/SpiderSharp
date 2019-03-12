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
        static HttpRequester.Requester requester;
        static string content;
        static CommandMenu currentMenu;

        static void LoadCommand(Match match)
        {
            var url = match.Groups["url"].Value;

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
        }

        static void SetCommand(Match match)
        {
            switch (match.Groups["driver"].Value)
            {
                case "anglesharp":
                    requester = new HttpRequester.Requester(HttpRequester.EnumHttpProvider.AngleSharp);
                    Console.WriteLine("AngleSharp has been set");
                    break;
            }
        }

        static void SaveCommand(Match match)
        {
            var filename = match.Groups["filename"].Value;

            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("You must load some url before execute save command");
            }
            else
            {
                System.IO.File.WriteAllText(filename, content);
                Console.WriteLine("Saved");
            }
        }

        static void HelpCommand(Match match)
        {
            currentMenu.PrintHelp();
        }

        static void BrowseCommand(Match match)
        {
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("You must load some content before execute open command");
            }
            else
            {
                var tmp = System.IO.Path.GetTempFileName() + ".html";
                System.IO.File.WriteAllText(tmp, content);
                OpenBrowser(tmp);

                Console.WriteLine("Browser opened");
            }
        }

        static void InnerTextCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                Nodes nodes = new Nodes(content);
                var res = nodes.SelectNodes(selector);
                res.ToList().ForEach(a => Console.WriteLine(a.GetInnerText()));
                Console.WriteLine($"{res.ToList().Count} itens found");
            }
        }

        static void InnerHtmlCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                Nodes nodes = new Nodes(content);
                var res = nodes.SelectNodes(selector);
                res.ToList().ForEach(a => Console.WriteLine(a.GetInnerHtml()));
                Console.WriteLine($"{res.ToList().Count} itens found");
            }
        }

        static void LinksCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                Nodes nodes = new Nodes(content);
                var res = nodes.SelectHref(selector);
                res.ToList().ForEach(a => Console.WriteLine(a));
                Console.WriteLine($"{res.ToList().Count} itens found");
            }
        }

        static void Main(string[] args)
        {
            var mainMenu = new CommandMenu("Main");
            mainMenu.AddCommand("set (?<driver>anglesharp)", SetCommand, "set => set HttpRequest that you want");
            mainMenu.AddCommand("load (?<url>.*)", LoadCommand, "load => load url or local file");
            mainMenu.AddCommand("save (?<filename>.*)", SaveCommand, "save => save content to local file");
            mainMenu.AddCommand("innertext (?<selector>.*)", InnerTextCommand, "innertext => select innertext using css selector");
            mainMenu.AddCommand("innerhtml (?<selector>.*)", InnerHtmlCommand, "innerhtml => select innerhtml using css selector");
            mainMenu.AddCommand("links (?<selector>.*)", LinksCommand, "links => select links using css selector");
            mainMenu.AddCommand("browse", BrowseCommand, "browse => open browser with content");
            mainMenu.AddCommand("help", HelpCommand, "help => show all commands");
            mainMenu.AddExitCommand("exit|quit|back", "quit => exit program");
            mainMenu.PrintHelp();
            currentMenu = mainMenu;
            mainMenu.Execute();
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

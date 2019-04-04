using AngleSharp.Dom;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace ScrapShell
{
    public class Program
    {
        static HttpRequester.RequesterCached requester;
        static string content;
        static IElement doc;
        static CommandMenu currentMenu;
        static string filename;

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
                    content = requester.GetAsync(url).Result.StringContent;
                }
                else
                {
                    HttpRequester.Engine.ChromePersistentClient httpRequester = new HttpRequester.Engine.ChromePersistentClient(true);
                    content = httpRequester.GetContentAsync(url).Result;
                }
            }

            filename = System.IO.Path.GetTempFileName() + ".txt";
            System.IO.File.WriteAllText(filename, content);

            doc = SpiderSharp.AngleDocument.TryParse(content)?.DocumentElement;

            var msg = $"{content.Count()} bytes loaded";
            var m  = nameof(msg);
            Console.WriteLine(msg);
        }

        static void SetCommand(Match match)
        {
            switch (match.Groups["driver"].Value)
            {
                case "anglesharp":
                    requester = new HttpRequester.RequesterCached(HttpRequester.EnumHttpProvider.AngleSharp);
                    Console.WriteLine("AngleSharp has been set");
                    break;

                case "httpclient":
                    requester = new HttpRequester.RequesterCached(HttpRequester.EnumHttpProvider.HttpClient);
                    Console.WriteLine("HttpClient has been set");
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
                if (string.IsNullOrEmpty(filename))
                {
                    filename = Path.GetTempFileName() + ".html";
                }

                System.IO.File.WriteAllText(filename, content);
                Console.WriteLine($"Saved {filename}");
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

        static void NotepadCommand(Match match)
        {
            if (string.IsNullOrEmpty(filename))
            {
                Console.WriteLine("You must load some content before execute open command");
            }
            else
            {
                OpenNotepad(filename);
                Console.WriteLine("notepad opened");
            }
        }

        static void ClearCommand(Match match)
        {
            Console.Clear();
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
                StringBuilder sb = new StringBuilder();

                var res = doc.QuerySelectorAll(selector);
                res.ToList().ForEach(a => sb.AppendLine(a.TextContent));
                sb.AppendLine($"{res.Count()} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".txt";
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }

        static void AttributeCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                var att = match.Groups["attribute"].Value;

                StringBuilder sb = new StringBuilder();
                var res = doc.QuerySelectorAll(selector);
                res.Where(w => w.HasAttribute(att)).ToList().ForEach(a => sb.AppendLine(a.Attributes[att].Value));
                sb.AppendLine($"{res.Where(w => w.HasAttribute(att)).Count()} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".txt";
                System.IO.File.WriteAllText(filename, sb.ToString());
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
                StringBuilder sb = new StringBuilder();

                var res = doc.QuerySelectorAll(selector);
                res.ToList().ForEach(a => sb.AppendLine(a.InnerHtml));
                sb.AppendLine($"{res.Count()} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".txt";
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }

        static void OuterHtmlCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                var res = doc.QuerySelectorAll(selector);
                res.ToList().ForEach(a => sb.AppendLine(a.OuterHtml));
                sb.AppendLine($"{res.Count()} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".txt";
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }

        static void JsonCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                var res = doc.QuerySelectorAll(selector);

                JArray arr = new JArray();
                var sb = new StringBuilder();
                res.ToList().ForEach(a => {
                    var json = a.GetJson(false);
                    arr.Add(json);
                    sb.AppendLine(json.ToString());
                });

                sb.AppendLine($"{res.ToList().Count} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".json";
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }

        static void PathCommand(Match match)
        {
            var selector = match.Groups["selector"].Value;
            if (string.IsNullOrEmpty(selector))
            {
                Console.WriteLine("You must type the selector");
            }
            else
            {
                var res = doc.QuerySelectorAll(selector);

                JArray arr = new JArray();
                var sb = new StringBuilder();
                res.ToList().ForEach(a => {
                    var json = a.GetJson(true);
                    arr.Add(json);
                    sb.AppendLine(json.ToString());
                });

                sb.AppendLine($"{res.ToList().Count} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".json";
                System.IO.File.WriteAllText(filename, sb.ToString());
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
                StringBuilder sb = new StringBuilder();

                var res = doc.QuerySelectorAll(selector);
                res.ToList().ForEach(a => sb.AppendLine(a.ToString()));
                sb.AppendLine($"{res.Count()} itens found");
                Console.WriteLine(sb.ToString());

                filename = Path.GetTempFileName() + ".txt";
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }
        
        static void Main(string[] args)
        {
            ReadLine.AutoCompletionHandler = new AutoCompletionHandler();

            var mainMenu = new CommandMenu("Main");
            mainMenu.AddCommand("set (?<driver>anglesharp|httpclient)", SetCommand, "set => set httpclient or anglesharp driver to request");
            mainMenu.AddCommand("load (?<url>.*)", LoadCommand, "load => load url or local file");
            mainMenu.AddCommand("save (?<filename>.*)", SaveCommand, "save => save content to local file");
            mainMenu.AddCommand("innertext (?<selector>.*)", InnerTextCommand, "innertext => select innertext using css selector");
            mainMenu.AddCommand("attribute (?<attribute>.*?) (?<selector>.*)", AttributeCommand, "attribute => select attribute value using css selector");
            mainMenu.AddCommand("innerhtml (?<selector>.*)", InnerHtmlCommand, "innerhtml => select innerhtml using css selector");
            mainMenu.AddCommand("outerhtml (?<selector>.*)", OuterHtmlCommand, "outerhtml => select outerhtml using css selector");
            mainMenu.AddCommand("links (?<selector>.*)", LinksCommand, "links => select links using css selector");
            mainMenu.AddCommand("json (?<selector>.*)", JsonCommand, "json => select links using css selector");
            mainMenu.AddCommand("path (?<selector>.*)", PathCommand, "path => select links using css selector and print json path");
            mainMenu.AddCommand("cls|clear", ClearCommand, "cls => clear screen");
            mainMenu.AddCommand("browse", BrowseCommand, "browse => open browser with content");
            mainMenu.AddCommand("notepad", NotepadCommand, "notepad => open notepad with content");
            mainMenu.AddCommand("help", HelpCommand, "help => show all commands");
            mainMenu.AddExitCommand("exit|quit|back", "quit => exit program");
            mainMenu.PrintHelp();
            Console.WriteLine(string.Empty);
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

        static void OpenNotepad(string filename)
        {
            bool hasRegistry = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (hasRegistry)
            {
                Process.Start(@"c:\windows\explorer.exe", filename);
            }
            else
            {
                System.Console.WriteLine("Platform not supported");
            }
        }
    }

    class AutoCompletionHandler : IAutoCompleteHandler
    {
        // characters to start completion from
        public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

        // text - The current text entered in the console
        // index - The index of the terminal cursor within {text}
        public string[] GetSuggestions(string text, int index)
        {
            if (text.StartsWith("set "))
                return new string[] { "anglesharp", "httpclient" };
            else
                return null;
        }
    }
}

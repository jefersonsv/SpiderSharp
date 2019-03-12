using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScrapShell
{
    class CommandItem
    {
        public Regex Regex { get; set; }
        public Action<Match> Action { get; set; }
        public string Description { get; set; }
    }

    public class CommandMenu
    {
        Dictionary<string, CommandItem> dict;
        string prefix;
        public delegate void CommandNotFound();
        public event CommandNotFound OnCommandNotFound;

        public CommandMenu(string menuName)
        {
            prefix = menuName;
            dict = new Dictionary<string, CommandItem>();
        }

        private void CommandMenu_OnCommandNotFound()
        {
            Console.WriteLine("Command not found");
        }

        public void AddCommand(string regex, Action<Match> action, string description = null)
        {
            if (action == null)
                throw new Exception("The action cannot be null");

            dict.Add(regex, new CommandItem { Regex = new Regex(regex, RegexOptions.IgnoreCase), Action = action, Description = description });
        }

        public void AddExitCommand(string regex, string description = null)
        {
            dict.Add(regex, new CommandItem { Regex = new Regex(regex, RegexOptions.IgnoreCase), Action = null, Description = description });
        }

        public void PrintHelp(char separator = '=')
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                Console.WriteLine(prefix);
                Console.WriteLine(new String(separator, prefix.Length));
            }

            foreach (var item in dict)
            {
                Console.WriteLine(item.Value.Description ?? item.Key);
            }
        }

        public void Execute()
        {
            Console.WriteLine(string.Empty);
            ReadLine.HistoryEnabled = true;

            while (true)
            {
                var isValid = false;
                string input = ReadLine.Read($"{prefix} # ");

                foreach (var item in dict)
                {
                    if (item.Value.Regex.IsMatch(input))
                    {
                        isValid = true;
                        if (item.Value.Action == null)
                            return;

                        var match = item.Value.Regex.Match(input);
                        try
                        {
                            item.Value.Action(match);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (!isValid)
                    {
                        if (OnCommandNotFound == null)
                            OnCommandNotFound += CommandMenu_OnCommandNotFound;

                        OnCommandNotFound();
                    }

                    Console.WriteLine(string.Empty);
                }
            }
        }
    }
}

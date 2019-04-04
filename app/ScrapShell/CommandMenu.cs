using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

            if (CheckCollide(regex))
                throw new Exception($"The '{regex}' will collide has other command");

            var reg = new Regex(regex, RegexOptions.IgnoreCase);
            dict.Add(regex, new CommandItem { Regex = reg, Action = action, Description = description });
        }

        public static string StripNameGroups(string regex)
        {
            return Regex.Replace(regex, @"\?<.*?>", String.Empty);
        }

        bool Collide(string regex1, string regex2)
        {
            var regexStriped1 = StripNameGroups(regex1);
            var regexStriped2 = StripNameGroups(regex2);

            var automaton1 = new Fare.RegExp(regexStriped1).ToAutomaton();
            var automaton2 = new Fare.RegExp(regexStriped2).ToAutomaton();

            var intersection = automaton1.Intersection(automaton2);

            var random1 = new Random(Environment.TickCount);
            var sut1 = new Fare.Xeger(regexStriped1, random1);

            var random2 = new Random(Environment.TickCount);
            var sut2 = new Fare.Xeger(regexStriped2, random2);

            var gen1 = sut1.Generate();
            var gen2 = sut2.Generate();

            var result1 = intersection.Run(gen1);
            var result2 = intersection.Run(gen2);

            return result1 || result2;
        }

        bool CheckCollide(string regex)
        {
            foreach (var item in dict)
            {
                if (Collide(item.Key, regex))
                    return true;
            }

            return false;
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
            ReadLine.HistoryEnabled = true;

            while (true)
            {
                var isValid = false;

                var restoreColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{prefix} # ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = ReadLine.Read();

                Console.ForegroundColor = restoreColor;

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

                    //Console.WriteLine(string.Empty);   
                }
            }
        }
    }
}

using System.Text;
using Cmaner.Holder;

namespace Cmaner
{
    internal static class Program
    {
        private static bool IsRunning { get; set; }
        private static Storage Storage { get; }

        static Program()
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ??
                          throw new IOException("Could not get executable path");
            var exeDir = Path.GetDirectoryName(exePath) ??
                         throw new IOException("Could not get executable directory");
            Storage = new Storage(Path.Combine(exeDir, "commands.dat"));
        }

        private static Command Menu()
        {
            Console.CursorVisible = false;
            var startCursorTop = Console.CursorTop;
            var screenBuffer = new List<string>(10) { "" };
            var tempBuffer = new List<string>(10);
            var selector = 0;
            Command? selectedCommand = null;
            while (true)
            {
                PrepareMenu(tempBuffer, ref selector, out var totalMenuCount, ref selectedCommand);
                WriteBuffer(screenBuffer, tempBuffer, startCursorTop);
                screenBuffer.Clear();
                screenBuffer.AddRange(tempBuffer);
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        selector--;
                        break;
                    case ConsoleKey.DownArrow:
                        selector++;
                        break;
                    case ConsoleKey.Enter:
                        if (selectedCommand != null)
                        {
                            Console.CursorVisible = true;
                            Console.WriteLine();
                            return selectedCommand;
                        }
                        break;
                }

                selector = Math.Clamp(selector, 0, totalMenuCount - 1);
            }
        }

        private static void PrepareMenu(List<string> buffer, ref int selector, out int totalMenuCount,
            ref Command? selectedCommand)
        {
            buffer.Clear();
            buffer.Add("==Select a command==");
            var curMenu = 0;
            foreach (var cat in Storage.Categories)
            {
                buffer.Add($"[{cat.Name}]");
                foreach (var cmd in cat.Commands)
                {
                    string selected;
                    if (selector == curMenu)
                    {
                        selected = ">> ";
                        selectedCommand = cmd;
                    }
                    else
                        selected = "  ";

                    var strBuilder = new StringBuilder(selected);
                    if (cmd.AdminRequired)
                        strBuilder.Append("[ADMIN] ");
                    if (!string.IsNullOrEmpty(cmd.Title))
                    {
                        strBuilder.Append(cmd.Title);
                        strBuilder.Append($" ({cmd.CommandText})");
                    }
                    else
                        strBuilder.Append(cmd.CommandText);

                    if (!string.IsNullOrEmpty(cmd.Description))
                        strBuilder.Append($" - {cmd.Description}");
                    buffer.Add($"\t{strBuilder}");
                    curMenu++;
                }
            }

            totalMenuCount = curMenu;
        }

        private static void WriteBuffer(List<string> oldScreen, List<string> newScreen, int top)
        {
            Console.SetCursorPosition(0, top);
            var oldMax = oldScreen.Max(x => x.Length);
            foreach (var line in newScreen)
            {
                var clearLine = new string(' ', Math.Clamp(oldMax - line.Length, 0, oldMax));
                Console.WriteLine(line + clearLine);
            }
        }

        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += OnConsoleOnCancelKeyPress;


            /*
         Storage.Categories.Add(new Category
             {
                 Name = "First",
                 Description = "First category",
                 Commands = new List<Command>
                 {
                     new()
                     {
                         Title = "Just a command",
                         Description = "This is a command",
                         CommandText = "echo Hello World",
                         AdminRequired = false,
                         ShortCall = "hello"
                     },
                     new()
                     {
                         Title = "One more command",
                         Description = "This is another command",
                         CommandText = "echo Aboba",
                         AdminRequired = false,
                         ShortCall = "aboba"
                     }
                 }
             }
         );
         Storage.Categories.Add(new Category
             {
                 Name = "Second",
                 Commands = new List<Command>
                 {
                     new()
                     {
                         Title = "Long command",
                         Description = "This is a long command",
                         CommandText = "ping 1.1.1.1",
                         AdminRequired = false,
                         ShortCall = "pn1"
                     },
                     new()
                     {
                         Title = "Another long command",
                         Description = null,
                         CommandText = "mkdir test",
                         AdminRequired = true,
                         ShortCall = "mkd"
                     }
                 }
             }
         );
         Storage.Save();
        // */
            var command =  Menu();
            var runner = new Runner(command);
            try
            {
                IsRunning = true;
                await runner.RunAsync();
                IsRunning = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            Console.WriteLine("Done");
        }

        private static void OnConsoleOnCancelKeyPress(object? _, ConsoleCancelEventArgs eventArgs)
        {
            if (IsRunning)
                eventArgs.Cancel = true;
        }
    }
}
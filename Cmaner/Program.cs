using System.Diagnostics;
using System.Text;
using Cmaner.Holder;
using Cmaner.Menu;

namespace Cmaner
{
    internal static class Program
    {
        private static bool IsRunning { get; set; }
        private static Storage Storage { get; }

        static Program()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName ??
                          throw new IOException("Could not get executable path");
            var exeDir = Path.GetDirectoryName(exePath) ??
                         throw new IOException("Could not get executable directory");
            Storage = new Storage(Path.Combine(exeDir, "commands.dat"));
        }

        private static async Task RunCmd(Command cmd)
        {
            var runner = new Runner(cmd);
            try
            {
                IsRunning = true;
                Console.WriteLine($"Running {cmd.CommandText}");
                await runner.RunAsync();
                IsRunning = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task NoArgsCall()
        {
            var menu = new MenuCommand(Storage);
            var result = MenuRunner.RunMenu(menu);
            if (result == null)
            {
                Console.WriteLine("None");
                return;
            }

            await RunCmd(result);
        }

        private static void AddCategoryCall()
        {
            var cat = new Category();
            Console.Write("Enter category name: ");
            while (true)
            {
                var name = Console.ReadLine();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty");
                    continue;
                }

                cat.Name = name;
                break;
            }

            Console.Write("Enter description [optional]: ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
                cat.Description = desc;

            var menuConf = new MenuConfirm("Are you sure?");
            var result = MenuRunner.RunMenu(menuConf);
            if (result)
            {
                Console.WriteLine($"Added {cat.Name}");
                Storage.Categories.Add(cat);
                Storage.Save();
            }
            else
            {
                Console.WriteLine("Aborted");
            }
        }

        private static void HelpCall()
        {
            Console.WriteLine("Cmaner - Command Manager");
            Console.WriteLine("Usage:");
            Console.WriteLine("\t cm - Opens command menu");
            Console.WriteLine("\t cm add category - Adds a new category");
            Console.WriteLine("\t cm add command - Adds a new command");
            Console.WriteLine("\t cm help - Shows this help");
            Console.WriteLine("\t cm [shortcut] - Runs a command");
        }

        private static void AddCommandCall()
        {
            var catMenu = new MenuCategory(Storage);
            var cat = MenuRunner.RunMenu(catMenu);
            if (cat == null)
            {
                Console.WriteLine("Aborted");
                return;
            }

            Console.WriteLine($"Selected category {cat.Name}");
            var cmd = new Command();
            Console.Write("Enter command title [optional]: ");
            var title = Console.ReadLine();
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
                cmd.Title = title;

            Console.Write("Enter command text: ");
            while (true)
            {
                var text = Console.ReadLine();
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    Console.WriteLine("Command text cannot be empty");
                    continue;
                }

                cmd.CommandText = text;
                break;
            }

            Console.Write("Enter working directory [optional]: ");
            var dir = Console.ReadLine();
            if (!string.IsNullOrEmpty(dir) && !string.IsNullOrWhiteSpace(dir))
                cmd.WorkingDirectory = dir;

            Console.Write("Enter description [optional]: ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
                cmd.Description = desc;

            Console.Write("Enter short call [optional]: ");
            var shortCall = Console.ReadLine();
            if (!string.IsNullOrEmpty(shortCall) && !string.IsNullOrWhiteSpace(shortCall))
                cmd.ShortCall = shortCall;

            var admReq = new MenuConfirm("Admin required to run?");
            var result = MenuRunner.RunMenu(admReq);
            cmd.AdminRequired = result;
            Console.WriteLine($"Admin required: {(result ? "Yes" : "No")}");

            var menuConf = new MenuConfirm("Are you sure?");
            result = MenuRunner.RunMenu(menuConf);
            if (result)
            {
                Console.WriteLine($"Added {cmd.CommandText}");
                cat.Commands.Add(cmd);
                Storage.Save();
            }
            else
            {
                Console.WriteLine("Aborted");
            }
        }

        private static void RemoveCommandCall()
        {
            var catMenu = new MenuCommand(Storage);
            var cat = MenuRunner.RunMenu(catMenu);
            if (cat == null)
            {
                Console.WriteLine("Aborted");
                return;
            }

            var menuConf = new MenuConfirm("Are you sure?");
            var result = MenuRunner.RunMenu(menuConf);
            if (result)
            {
                var category = Storage.Categories.FirstOrDefault(x => x.Commands.Contains(cat));
                if (category == null)
                {
                    Console.WriteLine("Could not find category");
                    return;
                }
                category.Commands.Remove(cat);
                Storage.Save();
                Console.WriteLine($"Removed {cat.CommandText}");

            }
            else
            {
                Console.WriteLine("Aborted");
            }
        }

        private static void RemoveCategoryCall()
        {
            var catMenu = new MenuCategory(Storage);
            var cat = MenuRunner.RunMenu(catMenu);
            if (cat == null)
            {
                Console.WriteLine("Aborted");
                return;
            }

            var commandCount = cat.Commands.Count;
            var menuConf = new MenuConfirm($"Are you sure? it has {commandCount} commands");
            var result = MenuRunner.RunMenu(menuConf);
            if (result)
            {
                Console.WriteLine($"Removed {cat.Name}");
                Storage.Categories.Remove(cat);
                Storage.Save();
            }
            else
            {
                Console.WriteLine("Aborted");
            }
        }

        private static bool HasShortCall(string shortCall, out Command? cmd)
        {
            var commands = Storage.Categories.SelectMany(x => x.Commands);
            foreach (var command in commands)
            {
                if (command.ShortCall != shortCall) continue;
                cmd = command;
                return true;
            }

            cmd = null;
            return false;
        }

        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;
            switch (args)
            {
                case []:
                    await NoArgsCall();
                    break;
                case ["add"]:
                    Console.WriteLine("What do you want to add? (category), (command)");
                    break;
                case ["rm"]:
                    Console.WriteLine("What do you want to delete? (category), (command)");
                    break;
                case ["add", "category"]:
                    AddCategoryCall();
                    break;
                case ["rm", "category"]:
                    RemoveCategoryCall();
                    break;
                case ["add", "command"]:
                    AddCommandCall();
                    break;
                case ["rm", "command"]:
                    RemoveCommandCall();
                    break;
                case ["help"]:
                    HelpCall();
                    break;
                case [{ } shortCall] when HasShortCall(shortCall, out var cmd):
                    await RunCmd(cmd!);
                    break;
                default:
                    Console.WriteLine("Wrong arguments, write 'help' for help");
                    break;
            }
        }

        private static void OnConsoleOnCancelKeyPress(object? _, ConsoleCancelEventArgs eventArgs)
        {
            if (IsRunning)
                eventArgs.Cancel = true;
        }
    }
}
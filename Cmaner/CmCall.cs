using System.Text;
using Cmaner.Holder;
using Cmaner.Menu;

namespace Cmaner;

/// <summary>
/// A class of methods representing commands for CM
/// </summary>
public static class CmCall
{
    /// <summary>
    /// Run a command and block console until it's done
    /// </summary>
    /// <param name="cmd">Command to run</param>
    public static async Task RunCmd(Command cmd)
    {
        var runner = new Runner(cmd);
        try
        {
            Console.WriteLine($"Running {cmd.CommandText}");
            CmConfig.CanBeInterrupted = false;
            await runner.RunAsync();
            CmConfig.CanBeInterrupted = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Run a command by displaying a menu of commands to the user and then executing the selected command
    /// </summary>
    public static async Task RunCommand()
    {
        if (CmStorage.Instance.Categories.Count == 0)
        {
            Console.WriteLine("No categories found, run [help] to see how to add one");
            return;
        }

        if (!CmStorage.Instance.Categories.SelectMany(x => x.Commands).Any())
        {
            Console.WriteLine("No commands found, run [help] to see how to add one");
            return;
        }

        var result = MenuRunner.RunMenu(new MenuCommand());
        if (result != null)
            await RunCmd(result);
    }

    #region Category Management

    /// <summary>
    /// Add a new category by prompting the user
    /// </summary>
    public static void AddCategory()
    {
        var cat = new Category();
        while (true)
        {
            var name = ReadLine.Read("Enter category name: ").Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty");
                continue;
            }

            cat.Name = name;
            break;
        }

        var desc = ReadLine.Read("Enter description [optional]: ").Trim();
        if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
            cat.Description = desc;

        var menuConf = new MenuConfirm("Are you sure?");
        var result = MenuRunner.RunMenu(menuConf);
        if (result)
        {
            Console.WriteLine($"Added {cat.Name}");
            CmStorage.Instance.Categories.Add(cat);
            CmStorage.Instance.Save();
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    /// <summary>
    /// Remove a category by displaying
    /// </summary>
    public static void RemoveCategory()
    {
        var catMenu = new MenuCategory();
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
            CmStorage.Instance.Categories.Remove(cat);
            CmStorage.Instance.Save();
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    /// <summary>
    /// Edit a category by displaying a menu of categories to the user and then editing the selected category
    /// </summary>
    public static void EditCategory()
    {
        var cat = MenuRunner.RunMenu(new MenuCategory());
        if (cat == null)
        {
            Console.WriteLine("Aborted");
            return;
        }

        Console.WriteLine($"Editing {cat.Name}");
        var name = ReadLine.Read($"Enter new name [{cat.Name}]: ", cat.Name).Trim();
        var desc = ReadLine.Read($"Enter new description [{cat.Description}]: ", cat.Description).Trim();

        if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            cat.Name = name;
        if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
            cat.Description = desc;

        var menuConf = new MenuConfirm("Are you sure?");
        var result = MenuRunner.RunMenu(menuConf);
        if (result)
        {
            Console.WriteLine($"Edited {cat.Name}");
            CmStorage.Instance.Save();
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    #endregion

    #region Command Management

    /// <summary>
    /// Add a new command to a category by displaying a menu of categories
    /// </summary>
    public static void AddCommand()
    {
        var catMenu = new MenuCategory();
        var cat = MenuRunner.RunMenu(catMenu);
        if (cat == null)
        {
            Console.WriteLine("Aborted");
            return;
        }

        Console.WriteLine($"Selected category {cat.Name}");
        var cmd = new Command();
        var title = ReadLine.Read("Enter command title [optional]: ").Trim();
        
        while (true)
        {
            var text = ReadLine.Read("Enter command text: ").Trim();
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Command text cannot be empty");
                continue;
            }

            cmd.CommandText = text;
            break;
        }

        var dir = ReadLine.Read("Enter working directory [optional]: ").Trim();
        var desc = ReadLine.Read("Enter description [optional]: ").Trim();
        var shortCall = ReadLine.Read("Enter short call [optional]: ").Trim();

        if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
            cmd.Title = title;
        
        if (!string.IsNullOrEmpty(dir) && !string.IsNullOrWhiteSpace(dir))
            cmd.WorkingDirectory = dir;

        if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
            cmd.Description = desc;

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
            CmStorage.Instance.Save();
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    /// <summary>
    /// Remove a command from a category by displaying a menu of categories and then a menu of commands
    /// </summary>
    public static void RemoveCommand()
    {
        var catMenu = new MenuCommand();
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
            var category = CmStorage.Instance.Categories.FirstOrDefault(x => x.Commands.Contains(cat));
            if (category == null)
            {
                Console.WriteLine("Could not find category");
                return;
            }

            category.Commands.Remove(cat);
            CmStorage.Instance.Save();
            Console.WriteLine($"Removed {cat.CommandText}");
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    /// <summary>
    /// Edit a command by displaying a menu of categories and then a menu of commands
    /// </summary>
    public static void EditCommand()
    {
        var catMenu = new MenuCommand();
        var cat = MenuRunner.RunMenu(catMenu);
        if (cat == null)
        {
            Console.WriteLine("Aborted");
            return;
        }

        Console.WriteLine($"Editing {cat.CommandText}");

        var title = ReadLine.Read($"Enter new title [{cat.Title}]: ", cat.Title).Trim();
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
            cat.Title = title;

        var text = ReadLine.Read($"Enter new command text [{cat.CommandText}]: ", cat.CommandText).Trim();
        if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
            cat.CommandText = text;

      
        var dir = ReadLine.Read($"Enter new working directory [{cat.WorkingDirectory}]: ", cat.WorkingDirectory).Trim();
        if (!string.IsNullOrEmpty(dir) && !string.IsNullOrWhiteSpace(dir))
            cat.WorkingDirectory = dir;
        
        var desc = ReadLine.Read($"Enter new description [{cat.Description}]: ", cat.Description).Trim();
        if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc))
            cat.Description = desc;

        var shortCall = ReadLine.Read($"Enter new short call [{cat.ShortCall}]: ", cat.ShortCall).Trim();
        if (!string.IsNullOrEmpty(shortCall) && !string.IsNullOrWhiteSpace(shortCall))
            cat.ShortCall = shortCall;

        var admReq = new MenuConfirm("Admin required to run?");
        var result = MenuRunner.RunMenu(admReq);
        cat.AdminRequired = result;

        var menuConf = new MenuConfirm("Are you sure?");
        result = MenuRunner.RunMenu(menuConf);
        if (result)
        {
            Console.WriteLine($"Edited {cat.CommandText}");
            CmStorage.Instance.Save();
        }
        else
        {
            Console.WriteLine("Aborted");
        }
    }

    #endregion

    /// <summary>
    /// Help list call
    /// </summary>
    public static void Help()
    {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("Cmaner - command manager");
        strBuilder.AppendLine("Usage: ");
        strBuilder.AppendLine("cm - run menu");
        strBuilder.AppendLine("cm [add] [category] - add category");
        strBuilder.AppendLine("cm [add] [command] - add command");
        strBuilder.AppendLine("cm [rm] [category] - remove category");
        strBuilder.AppendLine("cm [rm] [command] - remove command");
        strBuilder.AppendLine("cm [edit] [category] - edit category");
        strBuilder.AppendLine("cm [edit] [command] - edit command");
        strBuilder.AppendLine("cm [help] - show this help");
        strBuilder.AppendLine("cm [short call] - run command");
        Console.WriteLine(strBuilder.ToString());
    }
}
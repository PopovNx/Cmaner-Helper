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
    /// <param name="lArg">Arguments to pass to command</param>
    public static async Task RunCmd(Command cmd, string[] lArg)
    {
        var runner = new Runner(cmd);
        try
        {
            var title = string.IsNullOrWhiteSpace(cmd.Title) ? cmd.CommandText : cmd.Title;

            string displayTitle;
            if (cmd.Flags.HasFlag(CmdFlags.HasTitle))
                displayTitle = $"{title}";
            else
                displayTitle = cmd.Flags.HasFlag(CmdFlags.HideCommandText)
                    ? @"command"
                    : $"({cmd.CommandText})";

            if (cmd.Flags.HasFlag(CmdFlags.RequestConfirmation))
            {
                var confirm = new MenuConfirm($"Are you sure you want to execute {displayTitle}?").RunS();
                if (!confirm)
                    return;
            }

            if (cmd.Flags.HasFlag(CmdFlags.RequestArguments) && lArg.Length == 0)
            {
                var args = ReadLine.Read($"Enter arguments for {displayTitle}: ");
                var addingArgs = args.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                lArg = lArg.Concat(addingArgs).ToArray();
            }

            if (!cmd.Flags.HasFlag(CmdFlags.SilentExecution))
                Console.WriteLine($"Running {displayTitle}...");
            CmConfig.CanBeInterrupted = false;
            await runner.RunAsync(lArg);
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

        var result = new MenuCommand().RunS();
        await RunCmd(result, Array.Empty<string>());
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
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty");
                continue;
            }

            cat.Name = name;
            break;
        }

        var desc = ReadLine.Read("Enter description [optional]: ").Trim();
        if (!string.IsNullOrWhiteSpace(desc))
            cat.Description = desc;

        if (!new MenuConfirm("Are you sure?").RunS())
            throw new OperationCanceledException();

        Console.WriteLine($"Added {cat.Name}");
        CmStorage.Instance.Categories.Add(cat);
        CmStorage.Instance.Save();
    }

    /// <summary>
    /// Remove a category by displaying
    /// </summary>
    public static void RemoveCategory()
    {
        var cat = new MenuCategory().RunS();

        var result = new MenuConfirm($"Are you sure? it has {cat.Commands.Count} commands").RunS();
        if (!result)
            throw new OperationCanceledException();

        Console.WriteLine($"Removed {cat.Name}");
        CmStorage.Instance.Categories.Remove(cat);
        CmStorage.Instance.Save();
    }

    /// <summary>
    /// Edit a category by displaying a menu of categories to the user and then editing the selected category
    /// </summary>
    public static void EditCategory()
    {
        var cat = new MenuCategory().RunS();

        Console.WriteLine($"Editing {cat.Name}");
        var name = ReadLine.Read($"Enter new name [{cat.Name}]: ", cat.Name).Trim();
        var desc = ReadLine.Read($"Enter new description [{cat.Description}]: ", cat.Description).Trim();

        if (!string.IsNullOrWhiteSpace(name))
            cat.Name = name;
        if (!string.IsNullOrWhiteSpace(desc))
            cat.Description = desc;

        if (!new MenuConfirm("Are you sure?").RunS())
            throw new OperationCanceledException("Aborted");

        Console.WriteLine($"Edited {cat.Name}");
        CmStorage.Instance.Save();
    }

    #endregion

    #region Command Management

    /// <summary>
    /// Add a new command to a category by displaying a menu of categories
    /// </summary>
    public static void AddCommand()
    {
        var category = new MenuCategory().RunS();

        Console.WriteLine($"Selected category {category.Name}");

        var title = ReadLine.Read("Enter command title [optional]: ").Trim();

        string commandText;
        while (true)
        {
            var text = ReadLine.Read("Enter command text: ").Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Command text cannot be empty");
                continue;
            }

            commandText = text;
            break;
        }

        var dir = ReadLine.Read("Enter working directory [optional]: ").Trim();
        var desc = ReadLine.Read("Enter description [optional]: ").Trim();
        var shortCall = ReadLine.Read("Enter short call [optional]: ").Trim();

        var flags = new MenuFlagsSelector(CmdFlags.None).Run();

        var sure = new MenuConfirm("Are you sure?").Run();

        if (!sure)
            throw new OperationCanceledException();

        var cmd = new Command
        {
            CommandText = commandText,
            Title = title,
            WorkingDirectory = dir,
            Description = desc,
            ShortCall = shortCall,
            Flags = flags
        };

        if (!string.IsNullOrWhiteSpace(title))
            cmd.Flags |= CmdFlags.HasTitle;

        if (!string.IsNullOrWhiteSpace(dir))
            cmd.Flags |= CmdFlags.HasWorkingDirectory;

        if (!string.IsNullOrWhiteSpace(desc))
            cmd.Flags |= CmdFlags.HasDescription;

        if (!string.IsNullOrWhiteSpace(shortCall))
            cmd.Flags |= CmdFlags.HasShortCall;


        Console.WriteLine($"Added {cmd.CommandText}");
        category.Commands.Add(cmd);
        CmStorage.Instance.Save();
    }

    /// <summary>
    /// Remove a command from a category by displaying a menu of categories and then a menu of commands
    /// </summary>
    public static void RemoveCommand()
    {
        var cat = new MenuCommand().RunS();
        var result = new MenuConfirm("Are you sure?").RunS();
        if (!result)
            throw new OperationCanceledException();

        var category = CmStorage.Instance.Categories.FirstOrDefault(x => x.Commands.Contains(cat));
        if (category == null)
        {
            Console.WriteLine("Could not find category");
            return;
        }

        category.Commands.Remove(cat);
        CmStorage.Instance.Save();
    }

    /// <summary>
    /// Edit a command by displaying a menu of categories and then a menu of commands
    /// </summary>
    public static void EditCommand()
    {
        var cat = new MenuCommand().RunS();

        Console.WriteLine($"Editing ({cat.CommandText})");

        var title = ReadLine.Read($"Enter new title [{cat.Title}]: ", cat.Title).Trim();

        if (string.IsNullOrWhiteSpace(title))
            cat.Flags &= ~CmdFlags.HasTitle;
        else
        {
            cat.Flags |= CmdFlags.HasTitle;
            cat.Title = title;
        }

        var text = ReadLine.Read($"Enter new command text [{cat.CommandText}]: ", cat.CommandText).Trim();
        if (!string.IsNullOrWhiteSpace(text))
            cat.CommandText = text;


        var dir = ReadLine.Read($"Enter new working directory [{cat.WorkingDirectory}]: ", cat.WorkingDirectory).Trim();

        if (!string.IsNullOrWhiteSpace(dir))
            cat.Flags |= CmdFlags.HasWorkingDirectory;
        else
        {
            cat.Flags &= ~CmdFlags.HasWorkingDirectory;
            cat.WorkingDirectory = dir;
        }

        var desc = ReadLine.Read($"Enter new description [{cat.Description}]: ", cat.Description).Trim();
        if (!string.IsNullOrWhiteSpace(desc))
            cat.Flags |= CmdFlags.HasDescription;
        else
        {
            cat.Flags &= ~CmdFlags.HasDescription;
            cat.Description = desc;
        }

        var shortCall = ReadLine.Read($"Enter new short call [{cat.ShortCall}]: ", cat.ShortCall).Trim();
        if (!string.IsNullOrWhiteSpace(shortCall))
            cat.Flags |= CmdFlags.HasShortCall;
        else
        {
            cat.Flags &= ~CmdFlags.HasShortCall;
            cat.ShortCall = shortCall;
        }

        cat.Flags = new MenuFlagsSelector(cat.Flags).Run();

        var result = new MenuConfirm("Are you sure?").Run();


        if (!result)
            throw new OperationCanceledException();
        Console.WriteLine($"Edited {cat.CommandText}");
        CmStorage.Instance.Save();
    }

    #endregion

    /// <summary>
    /// Help list call
    /// </summary>
    public static void Help()
    {
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("Cmaner - command manager v1.2.1");
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

    public static void InitDefault()
    {
        if (CmStorage.Instance.Categories.Count > 0)
            return;

        CmStorage.Instance.Categories.Clear();
        CmStorage.Instance.Save();

        var cat = new Category
        {
            Name = "Default",
            Description = "Default category"
        };

        var cmd1 = new Command
        {
            Title = "Echo",
            CommandText = "echo Hello world!",
            Description = "Echo command",
            ShortCall = "hi",
            Flags = CmdFlags.HasTitle | CmdFlags.HasDescription | CmdFlags.HasShortCall
        };

        var cmd2 = new Command
        {
            Title = "About",
            CommandText = @"echo Cmaner - command manager, written by @PopovDev, write ""cm help"" for more info",
            Description = "About command",
            ShortCall = "about",
            Flags = CmdFlags.HasTitle | CmdFlags.HasDescription | CmdFlags.HasShortCall | CmdFlags.Highlight |
                    CmdFlags.HideCommandText | CmdFlags.SilentExecution
        };

        cat.Commands.Add(cmd1);
        cat.Commands.Add(cmd2);

        CmStorage.Instance.Categories.Add(cat);
        CmStorage.Instance.Save();
        Console.WriteLine("Default commands added");
    }
}
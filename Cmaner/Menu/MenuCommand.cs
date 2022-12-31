using System.Text;
using Cmaner.Holder;

namespace Cmaner.Menu;

public class MenuCommand : IMenu<Command?>
{
    private readonly Storage _storage;
    private int _selected;
    private int _totalMenuItems;
    public MenuCommand(Storage storage) => _storage = storage;

    public IEnumerable<string> PrepareBuffer()
    {
        var curMenu = 0;
        yield return "== Select a command ==";
        foreach (var cat in _storage.Categories)
        {
            if (string.IsNullOrEmpty(cat.Description))
                yield return $"[{cat.Name}]";
            else
                yield return $"[{cat.Name}] - {cat.Description}";
            foreach (var cmd in cat.Commands)
            {
                if (_selected == curMenu)
                    Result = cmd;
                var strBuilder = new StringBuilder(_selected == curMenu ? ">> " : "  ");
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
                
                if (!string.IsNullOrEmpty(cmd.ShortCall))
                    strBuilder.Append($" (short: {cmd.ShortCall})");
                yield return $"\t{strBuilder}";
                curMenu++;
            }
            yield return "";
        }

        _totalMenuItems = curMenu;
    }


    public void ProcessInput()
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                _selected--;
                if (_selected < 0)
                    _selected = _totalMenuItems - 1;
                break;
            case ConsoleKey.DownArrow:
                _selected++;
                if (_selected >= _totalMenuItems)
                    _selected = 0;
                break;
            case ConsoleKey.Enter:
                IsFinished = true;
                break;
        }
    }

    public Command? Result { get; private set; }


    public bool IsFinished { get; private set; }
}
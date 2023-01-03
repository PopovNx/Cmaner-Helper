using System.Text;
using Cmaner.Holder;

namespace Cmaner.Menu;

public class MenuCommand : Menu<Command?>
{
    private int _selected;
    private int _totalMenuItems;

    public override IEnumerable<LineData> PrepareBuffer()
    {
        var curMenu = 0;
        yield return "== Select a command ==";

        foreach (var cat in CmStorage.Instance.Categories)
        {
            if (string.IsNullOrWhiteSpace(cat.Description))
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

                if (!string.IsNullOrWhiteSpace(cmd.Title))
                {
                    strBuilder.Append(cmd.Title);
                    strBuilder.Append($" ({cmd.CommandText})");
                }
                else
                    strBuilder.Append(cmd.CommandText);

                if (!string.IsNullOrWhiteSpace(cmd.Description))
                    strBuilder.Append($" - {cmd.Description}");

                if (!string.IsNullOrWhiteSpace(cmd.ShortCall))
                    strBuilder.Append($" (short: {cmd.ShortCall})");
                yield return $"\t{strBuilder}";
                curMenu++;
            }

            yield return "";
        }

        _totalMenuItems = curMenu;
    }


    public override void ProcessInput()
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (ReadKey().Key)
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
}
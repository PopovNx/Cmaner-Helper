using System.Text;
using Cmaner.Holder;

namespace Cmaner.Menu;

public class MenuCommand : Menu<Command>
{
    private int _selected;
    private int _selectedCat;
    private int _totalCatItems;

    public override IEnumerable<LineData> PrepareBuffer()
    {
        yield return "== Select a command ==";

        for (var ci = 0; ci < CmStorage.Instance.Categories.Count; ci++)
        {
            var cat = CmStorage.Instance.Categories[ci];
            var catSelected = ci == _selectedCat;


            var catBuilder = new StringBuilder();
            catBuilder.Append(catSelected ? ">> " : "   ");

            if (string.IsNullOrWhiteSpace(cat.Description))
                catBuilder.Append($"[{cat.Name}]");
            else
                catBuilder.Append($"[{cat.Name}] - {cat.Description}");
            if (catSelected)
            {
                yield return catBuilder.ToString().Color(ConsoleColor.Green);
                _totalCatItems = cat.Commands.Count;
            }
            else
                yield return catBuilder.ToString();

            for (var i = 0; i < cat.Commands.Count; i++)
            {
                var cmd = cat.Commands[i];
                var thisSelected = catSelected && i == _selected;
                if (thisSelected)
                    Result = cmd;

                var strBuilder = new StringBuilder();

                if (i < 9)
                    strBuilder.Append($"{i + 1}. ");
                else
                    strBuilder.Append(i == 9 ? $"0. " : $"*. ");

                strBuilder.Append(cmd.Flags.HasFlag(CmdFlags.AdminRequired) ? "!" : " ");

                strBuilder.Append(thisSelected ? "> " : " ");

                var cmdText = cmd.CommandText;
                if (cmd.Flags.HasFlag(CmdFlags.HideCommandText))
                    cmdText = "***";

                if (cmd.Flags.HasFlag(CmdFlags.HasTitle))
                {
                    strBuilder.Append(cmd.Title);
                    strBuilder.Append($" ({cmdText})");
                }
                else
                    strBuilder.Append(cmdText);

                if (cmd.Flags.HasFlag(CmdFlags.HasDescription))
                    strBuilder.Append($" - {cmd.Description}");

                if (cmd.Flags.HasFlag(CmdFlags.HasShortCall))
                    strBuilder.Append($" [сm {cmd.ShortCall}]");

                if (cmd.Flags.HasFlag(CmdFlags.Highlight))
                    yield return $" {strBuilder}".Color(ConsoleColor.Yellow);
                else
                    yield return $" {strBuilder}";
            }

            yield return "";
        }
    }


    public override void ProcessInput()
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        var key = ReadKey().Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                _selected--;
                if (_selected < 0)
                    _selected = _totalCatItems - 1;
                break;
            case ConsoleKey.DownArrow:
                _selected++;
                if (_selected >= _totalCatItems)
                    _selected = 0;
                break;
            case ConsoleKey.Tab:
                _selectedCat++;
                if (_selectedCat >= CmStorage.Instance.Categories.Count)
                    _selectedCat = 0;
                _selected = 0;
                break;
            case >= (ConsoleKey)0x30 and <= (ConsoleKey)0x39 or >= (ConsoleKey)0x60 and <= (ConsoleKey)0x69:
                var selector = key switch
                {
                    >= ConsoleKey.D0 and <= ConsoleKey.D9 => key - ConsoleKey.D0,
                    >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9 => key - ConsoleKey.NumPad0,
                    _ => throw new ArgumentOutOfRangeException()
                } - 1;
                if(selector == -1)
                    selector = 9;

                if (selector < CmStorage.Instance.Categories[_selectedCat].Commands.Count)
                {
                    _selected = selector;
                    Result = CmStorage.Instance.Categories[_selectedCat].Commands[_selected];
                    IsFinished = true;   
                }
                
                break;
            case ConsoleKey.Enter:
                IsFinished = true;
                break;
        }
    }
}
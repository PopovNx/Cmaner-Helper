using Cmaner.Holder;

namespace Cmaner.Menu;

public class MenuFlagsSelector : Menu<CmdFlags>
{
    public MenuFlagsSelector(CmdFlags flags) => Result = flags;

    private int _selected;
    private int _callNumber;

    private LineData SelectedMark(string text, CmdFlags flag)
    {
        var activated = (Result & flag) == flag;
        var color = activated ? ConsoleColor.Green : ConsoleColor.Red;
        var mark = activated ? "X" : " ";
        var prefix = _selected == _callNumber ? ">>" : "  ";
        var textWithMark = $"{_callNumber + 1}. {prefix}[{mark}] {text}";
        _callNumber++;
        return textWithMark.Color(color);
    }

    public override IEnumerable<LineData> PrepareBuffer()
    {
        yield return "Select flags: (use arrows to move, space or numbers to select, enter to confirm)".Color(
            ConsoleColor.Yellow);
        _callNumber = 0;
        yield return SelectedMark("Admin required", CmdFlags.AdminRequired);

        yield return SelectedMark("Silent execution", CmdFlags.SilentExecution);

        yield return SelectedMark("Request arguments before start", CmdFlags.RequestArguments);

        yield return SelectedMark("Request execution confirmation", CmdFlags.RequestConfirmation);

        yield return SelectedMark("Hide command text in menu", CmdFlags.HideCommandText);

        yield return SelectedMark("Highlight in menu", CmdFlags.Highlight);
        
    }

    public override void ProcessInput()
    {
        switch (ReadKey().Key)
        {
            case ConsoleKey.D1 or ConsoleKey.NumPad1:
            case ConsoleKey.Spacebar when _selected == 0:
                Result ^= CmdFlags.AdminRequired;
                break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2:
            case ConsoleKey.Spacebar when _selected == 1:
                Result ^= CmdFlags.SilentExecution;
                break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3:
            case ConsoleKey.Spacebar when _selected == 2:
                Result ^= CmdFlags.RequestArguments;
                break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4:
            case ConsoleKey.Spacebar when _selected == 3:
                Result ^= CmdFlags.RequestConfirmation;
                break;
            case ConsoleKey.D5 or ConsoleKey.NumPad5:
            case ConsoleKey.Spacebar when _selected == 4:
                Result ^= CmdFlags.HideCommandText;
                break;
            case ConsoleKey.D6 or ConsoleKey.NumPad6:
            case ConsoleKey.Spacebar when _selected == 5:
                Result ^= CmdFlags.Highlight;
                break;
            case ConsoleKey.UpArrow:
                _selected = _selected < 1 ? 5 : _selected - 1;
                break;
            case ConsoleKey.DownArrow:
                _selected = _selected > 4 ? 0 : _selected + 1;
                break;
            case ConsoleKey.Enter:
                IsFinished = true;
                break;
        }
    }
}
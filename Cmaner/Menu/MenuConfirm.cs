namespace Cmaner.Menu;

public class MenuConfirm : Menu<bool>
{
    public MenuConfirm(string message) => Message = message;

    private string Message { get; }

    public override IEnumerable<string> PrepareBuffer()
    {
        yield return Message;
        if (Result)
        {
            yield return "> Yes";
            yield return " No";
        }
        else
        {
            yield return " Yes";
            yield return "> No";
        }
    }

    public override void ProcessInput()
    {
        switch (ReadKey().Key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                Result = !Result;
                break;
            case ConsoleKey.Enter:
                IsFinished = true;
                break;
        }
    }

}
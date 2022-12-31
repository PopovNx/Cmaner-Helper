namespace Cmaner.Menu;

public class MenuConfirm : IMenu<bool>
{
    public MenuConfirm(string message) => Message = message;

    private string Message { get; }

    public IEnumerable<string> PrepareBuffer()
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

    public void ProcessInput()
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow or ConsoleKey.DownArrow:
                Result = !Result;
                break;
            case ConsoleKey.Enter:
                IsFinished = true;
                break;
        }
    }

    public bool Result { get; private set; }
    public bool IsFinished { get; private set; }
}
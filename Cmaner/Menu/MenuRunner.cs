namespace Cmaner.Menu;

public static class MenuRunner
{
    public static T RunMenu<T>(IMenu<T> menu)
    {
        Console.CursorVisible = false;
        var screenBuffer = new List<string>(16);
        Console.CancelKeyPress += (_, _) => Flush(screenBuffer);
        var tempBuffer = new List<string>(16);
        while (true)
        {
            tempBuffer.Clear();
            tempBuffer.AddRange(menu.PrepareBuffer());
            WriteBuffer(screenBuffer, tempBuffer);
            menu.ProcessInput();
            if (!menu.IsFinished) continue;
            Flush(screenBuffer);
            return menu.Result;
        }
    }

    private static void Flush(List<string> screenBuffer)
    {
        WriteBuffer(screenBuffer, Enumerable.Repeat("", screenBuffer.Count).ToList());
        Console.CursorVisible = true;
        WriteBuffer(screenBuffer, new List<string>());
    }

    private static void WriteBuffer(List<string> oldScreen, List<string> newScreen)
    {
        Console.SetCursorPosition(0, Console.CursorTop - oldScreen.Count);
        var clearLine = $"\r{new string(' ', Console.WindowWidth - 1)}\r";
        foreach (var line in newScreen)
        {
            Console.WriteLine(clearLine + line);
        }

        oldScreen.Clear();
        oldScreen.AddRange(newScreen);
    }
}
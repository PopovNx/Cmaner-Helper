namespace Cmaner.Menu;

public static class MenuRunner
{
    public static T? RunMenu<T>(Menu<T> menu)
    {
        Console.CursorVisible = false;
        var screenBuffer = new List<string>(16);
        var tempBuffer = new List<LineData>(16);
        try
        {
            while (true)
            {
                tempBuffer.Clear();
                tempBuffer.AddRange(menu.PrepareBuffer());
                WriteBuffer(screenBuffer, tempBuffer);

                menu.ProcessInput();
                if (!menu.IsFinished) continue;
                return menu.Result;
            }
        }
        catch (OperationCanceledException)
        {
            return default;
        }
        finally
        {
            Flush(screenBuffer);
            Console.CursorVisible = true;
        }
    }

    private static void Flush(List<string> screenBuffer)
    {
        WriteBuffer(screenBuffer, Enumerable.Repeat((LineData)"", screenBuffer.Count).ToList());
        Console.CursorVisible = true;
        WriteBuffer(screenBuffer, new List<LineData>());
    }


    private static void WriteBuffer(List<string> oldScreen, List<LineData> newScreen)
    {
        Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - oldScreen.Count));
        
        var foregroundColor = Console.ForegroundColor;
        var backgroundColor = Console.BackgroundColor;
        foreach (var line in newScreen)
        {
            if (line.ForegroundColor.HasValue)
                Console.ForegroundColor = line.ForegroundColor.Value;
            if (line.BackgroundColor.HasValue)
                Console.BackgroundColor = line.BackgroundColor.Value;
            
            var tabbedLine = line.Text.Replace("\t", "    ").PadRight(Console.WindowWidth);
            if (tabbedLine.Length > Console.WindowWidth)
                tabbedLine = $"{tabbedLine[..(Console.WindowWidth - 3)]}...";
            Console.WriteLine(tabbedLine);


            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
        }

        oldScreen.Clear();
        oldScreen.AddRange(newScreen.Select(x => x.ToString()));
    }
}
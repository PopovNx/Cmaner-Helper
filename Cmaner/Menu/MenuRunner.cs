namespace Cmaner.Menu;

public static class MenuRunner
{
    public static T? Run<T>(this Menu<T> menu)
    {
        Console.CursorVisible = false;
        var lastScreenLength = 0;
        var tempBuffer = new List<LineData>(16);
        try
        {
            while (true)
            {
                tempBuffer.Clear();
                tempBuffer.AddRange(menu.PrepareBuffer());
                WriteBuffer(ref lastScreenLength, tempBuffer);

                menu.ProcessInput();
                if (!menu.IsFinished) continue;
                return menu.Result;
            }
        }
        finally
        {
            Flush(ref lastScreenLength);
            Console.CursorVisible = true;
        }
    }

    
    public static T RunS<T>(this Menu<T> menu) =>
        menu.Run() ?? throw new NullReferenceException("Menu result is null");


    private static void Flush(ref int oldScreenLen)
    {
        WriteBuffer(ref oldScreenLen, Enumerable.Repeat((LineData)"", oldScreenLen).ToList());
        Console.CursorVisible = true;
        WriteBuffer(ref oldScreenLen, new List<LineData>());
    }


    private static void WriteBuffer(ref int oldScreenLen, List<LineData> newScreen)
    {
        Console.SetCursorPosition(0, Math.Max(0, Console.CursorTop - oldScreenLen));

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
        
        oldScreenLen = newScreen.Count;
    }
}
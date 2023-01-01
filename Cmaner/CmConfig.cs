using System.Text;

namespace Cmaner;

public static class CmConfig
{
    public static bool CanBeInterrupted { get; set; }

    public static void Init()
    {
        CanBeInterrupted = true;
        Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
    }

    private static void OnConsoleOnCancelKeyPress(object? _, ConsoleCancelEventArgs eventArgs)
    {
        if (CanBeInterrupted)
            Environment.Exit(0);
        else
            eventArgs.Cancel = true;
    }
}
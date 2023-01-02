using System.Text;

namespace Cmaner;

public static class CmConfig
{
    public static bool CanBeInterrupted { get; set; }

    public static void Init()
    {
        CanBeInterrupted = true;
        Console.TreatControlCAsInput = false;
        Console.CancelKeyPress += OnConsoleOnCancelKeyPress;
#if OS_WINDOWS
        Console.InputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.Unicode;
#else
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
#endif
    }

    private static void OnConsoleOnCancelKeyPress(object? _, ConsoleCancelEventArgs eventArgs)
    {
        if (CanBeInterrupted)
            Environment.Exit(0);
        else
            eventArgs.Cancel = true;
    }
}
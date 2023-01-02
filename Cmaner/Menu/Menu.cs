namespace Cmaner.Menu;

public abstract class Menu<T>
{
    public abstract IEnumerable<LineData> PrepareBuffer();
    public abstract void ProcessInput();
    public T? Result { get; protected set; }
    public bool IsFinished { get; protected set; }

    protected ConsoleKeyInfo ReadKey()
    {
        Console.TreatControlCAsInput = true;
        var key = Console.ReadKey(true);
        Console.TreatControlCAsInput = false;
        if (key is { Key: ConsoleKey.C, Modifiers: ConsoleModifiers.Control })
            throw new OperationCanceledException();
        return key;
    }
}
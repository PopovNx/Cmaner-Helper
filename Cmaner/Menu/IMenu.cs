namespace Cmaner.Menu;

public interface IMenu<out T>
{
    public IEnumerable<string> PrepareBuffer();
    public void ProcessInput();
    public T Result { get; }
    public bool IsFinished { get; }
}
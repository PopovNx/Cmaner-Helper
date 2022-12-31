using Cmaner.Holder;
namespace Cmaner.Menu;

public class MenuCategory : IMenu<Category?>
{
    private readonly Storage _storage;
    private int _selectedItem;

    public MenuCategory(Storage storage)
    {
        _storage = storage;
    }

    public IEnumerable<string> PrepareBuffer()
    {
        yield return "== Select category ==";
        for (var i = 0; i < _storage.Categories.Count; i++)
        {
            var category = _storage.Categories[i];
            var selected = i == _selectedItem ? ">>" : " ";
            if (string.IsNullOrEmpty(category.Description))
                yield return $"{selected} {category.Name}";
            else
                yield return $"{selected} {category.Name} - ({category.Description})";
        }
    }

    public void ProcessInput()
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.UpArrow:
                _selectedItem--;
                break;
            case ConsoleKey.DownArrow:
                _selectedItem++;
                break;
            case ConsoleKey.Enter:
                var category = _storage.Categories[_selectedItem];
                Result = category;
                IsFinished = true;
                break;
        }
        _selectedItem = Math.Clamp(_selectedItem, 0, _storage.Categories.Count - 1);
    }

    public Category? Result { get; private set; }
    public bool IsFinished { get; private set; }
}
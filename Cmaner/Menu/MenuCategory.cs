using Cmaner.Holder;

namespace Cmaner.Menu;

public class MenuCategory : Menu<Category?>
{
    private int _selectedItem;

    public override IEnumerable<LineData> PrepareBuffer()
    {
        yield return "== Select category ==";
        for (var i = 0; i < CmStorage.Instance.Categories.Count; i++)
        {
            var category = CmStorage.Instance.Categories[i];
            var selected = i == _selectedItem ? ">>" : " ";
            if (string.IsNullOrEmpty(category.Description))
                yield return $"{selected} {category.Name}";
            else
                yield return $"{selected} {category.Name} - ({category.Description})";
        }
    }

    public override void ProcessInput()
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (ReadKey().Key)
        {
            case ConsoleKey.UpArrow:
                _selectedItem--;
                break;
            case ConsoleKey.DownArrow:
                _selectedItem++;
                break;
            case ConsoleKey.Enter:
                var category = CmStorage.Instance.Categories[_selectedItem];
                Result = category;
                IsFinished = true;
                break;
        }

        _selectedItem = Math.Clamp(_selectedItem, 0, CmStorage.Instance.Categories.Count - 1);
    }
}
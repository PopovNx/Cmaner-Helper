using System.Text;

namespace Cmaner.Holder;

public class Storage
{
    public List<Category> Categories { get; }
    private string FileName { get; }

    public Storage(string filename)
    {
        if (!File.Exists(filename))
            File.Create(filename).Close();

        Categories = new List<Category>();
        FileName = filename;
        Load();
    }

    public void Save()
    {
        using var file = File.Open(FileName, FileMode.Truncate, FileAccess.Write);
        using var writer = new BinaryWriter(file, Encoding.UTF8);
        writer.Write(Categories.Count);
        foreach (var cat in Categories.Select(category => category.ToBytes()))
        {
            writer.Write(cat.Length);
            writer.Write(cat);
        }
    }

    private void Load()
    {
        using var file = File.Open(FileName, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(file, Encoding.UTF8);
        if (reader.BaseStream.Length == 0)
            return;
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            Categories.Add(Category.FromBytes(bytes));
        }
    }
}
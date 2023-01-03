namespace Cmaner.Holder;

public class Category
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<Command> Commands { get; private init; } = new();

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Name);
        writer.Write(Description ?? "");
        writer.Write(Commands.Count);
        foreach (var bytes in Commands.Select(command => command.ToBytes()))
        {
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        return ms.ToArray();
    }

    public static Category FromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var reader = new BinaryReader(ms);
        var category = new Category
        {
            Name = reader.ReadString(),
            Description = reader.ReadString(),
            Commands = new List<Command>()
        };
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            var length = reader.ReadInt32();
            var commandBytes = reader.ReadBytes(length);
            category.Commands.Add(Command.FromBytes(commandBytes));
        }

        return category;
    }
}
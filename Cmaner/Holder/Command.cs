namespace Cmaner.Holder;

public class Command
{
    public string? Title { get; set; }
    public bool AdminRequired { get; set; }
    public string CommandText { get; set; } = null!;
    public string? WorkingDirectory { get; set; }
    public string? Description { get; set; }
    public string? ShortCall { get; set; }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Title ?? "");
        writer.Write(AdminRequired);
        writer.Write(CommandText);
        writer.Write(WorkingDirectory ?? "");
        writer.Write(Description?? "");
        writer.Write(ShortCall ?? "");
        return ms.ToArray();
    }

    public static Command FromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var reader = new BinaryReader(ms);
        return new Command
        {
            Title = reader.ReadString(),
            AdminRequired = reader.ReadBoolean(),
            CommandText = reader.ReadString(),
            WorkingDirectory = reader.ReadString(),
            Description = reader.ReadString(),
            ShortCall = reader.ReadString()
        };
    }
}
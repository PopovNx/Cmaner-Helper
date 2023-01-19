namespace Cmaner.Holder;

public class Command
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public required string CommandText { get; set; }
    public CmdFlags Flags { get; set; }
    public string? WorkingDirectory { get; set; }
    public string? ShortCall { get; set; }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(Title ?? string.Empty);
        writer.Write(Description ?? string.Empty);
        writer.Write(CommandText);
        writer.Write((int)Flags);
        writer.Write(WorkingDirectory ?? string.Empty);
        writer.Write(ShortCall ?? string.Empty);
        return ms.ToArray();
    }

    public static Command FromBytes(byte[] bytes)
    {
        using var ms = new MemoryStream(bytes);
        using var reader = new BinaryReader(ms);
        return new Command
        {
            Title = reader.ReadString(),
            Description = reader.ReadString(),
            CommandText = reader.ReadString(),
            Flags = (CmdFlags)reader.ReadInt32(),
            WorkingDirectory = reader.ReadString(),
            ShortCall = reader.ReadString()
        };
    }
}
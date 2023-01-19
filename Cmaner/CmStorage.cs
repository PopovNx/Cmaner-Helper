using Cmaner.Holder;

namespace Cmaner;

public static class CmStorage
{
    public static Storage Instance { get; private set; } = null!;
    private const string StorageFileName = "commands.v2.dat";
    private const string StorageDirName = ".cm";

    /// <summary>
    /// Init storage with default path and\or create new storage file
    /// </summary>
    public static void Init()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var dataPath = Path.Combine(home, StorageDirName);
        if (!Directory.Exists(dataPath))
            Directory.CreateDirectory(dataPath);
        var storageFile = Path.Combine(dataPath, StorageFileName);
        Instance = new Storage(storageFile);
    }

    public static bool HasShortCall(string shortCall, out Command? cmd)
    {
        var commands = Instance.Categories.SelectMany(x => x.Commands)
            .Where(x => (x.Flags & CmdFlags.HasShortCall) != 0);
        foreach (var command in commands)
        {
            if (command.ShortCall != shortCall) continue;
            cmd = command;
            return true;
        }

        cmd = null;
        return false;
    }
}
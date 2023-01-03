namespace Cmaner;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        CmConfig.Init();
        CmStorage.Init();
        switch (args)
        {
            case []:
                await CmCall.RunCommand();
                break;
            case ["add"]:
                Console.WriteLine("What do you want to add (category/command)?");
                break;
            case ["rm"]:
                Console.WriteLine("What do you want to delete (category/command)?");
                break;
            case ["add", "category"]:
                CmCall.AddCategory();
                break;
            case ["rm", "category"]:
                CmCall.RemoveCategory();
                break;
            case ["add", "command"]:
                CmCall.AddCommand();
                break;
            case ["rm", "command"]:
                CmCall.RemoveCommand();
                break;
            case ["help"]:
                CmCall.Help();
                break;
            case [{ } shortCall] when CmStorage.HasShortCall(shortCall, out var cmd):
                await CmCall.RunCmd(cmd ?? throw new NullReferenceException("Cmd is null"));
                break;
            default:
                Console.WriteLine("Wrong arguments, write [help] for help");
                break;
        }
    }
}
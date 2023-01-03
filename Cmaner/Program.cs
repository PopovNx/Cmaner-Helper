global using ReadLineReboot;

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
            case ["edit"]:
                Console.WriteLine("What do you want to edit (category/command)?");
                break;
            case ["add", "category"]:
                CmCall.AddCategory();
                break;
            case ["rm", "category"]:
                CmCall.RemoveCategory();
                break;
            case ["edit", "category"]:
                CmCall.EditCategory();
                break;
            case ["add", "command"]:
                CmCall.AddCommand();
                break;
            case ["rm", "command"]:
                CmCall.RemoveCommand();
                break;
            case ["edit", "command"]:
                CmCall.EditCommand();
                break;
            case ["@initialize@"]:
                CmCall.InitDefault();
                break;
            case ["help"]:
                CmCall.Help();
                break;
            case [{ } shortCall, .. { } lArg] when CmStorage.HasShortCall(shortCall, out var cmd):
                await CmCall.RunCmd(cmd ?? throw new NullReferenceException("Cmd is null"), lArg);
                break;
            default:
                Console.WriteLine("Wrong arguments, write [help] for help");
                break;
        }
    }
}
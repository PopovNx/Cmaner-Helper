using System.Diagnostics;
using System.Text.Json.Serialization;
using Cmaner.Holder;
#if OS_WINDOWS
using System.Security.Principal;
#endif
namespace Cmaner;

public class Runner
{
    private Command Command { get; }

    private ProcessStartInfo GetProcessStartInfo()
    {
#if OS_WINDOWS
        if (Command.AdminRequired)
        {
#pragma warning disable CA1416 // Validate platform compatibility

            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return new ProcessStartInfo
                {
                    WorkingDirectory = Command.WorkingDirectory ?? Environment.CurrentDirectory,
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{Command.CommandText}\"",
                    Verb = "runas",
                    UseShellExecute = true
                };
            }
        }

        return new ProcessStartInfo("cmd.exe", $"/c \"{Command.CommandText}\"");
#elif OS_LINUX
        return Command.AdminRequired
            ? new ProcessStartInfo("bash", $"-c \"sudo {Command.CommandText}\"")
            : new ProcessStartInfo("bash", "-c \"" + Command.CommandText + "\"");
#elif OS_MAC
        return Command.AdminRequired
            ? new ProcessStartInfo("bash", $"-c \"sudo {Command.CommandText}\"")
            : new ProcessStartInfo("bash", "-c \"" + Command.CommandText + "\"");
#else
    #error Unsupported OS
#endif
    }

    public Runner(Command command) => Command = command;

    public async Task RunAsync()
    {
        var process = new Process
        {
            StartInfo = GetProcessStartInfo()
        };
        process.Start();
        await process.WaitForExitAsync();
    }
}
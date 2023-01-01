using System.Diagnostics;
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
        var cmd = Command.CommandText.Replace("\"", "\\\"");
        var dir = Command.WorkingDirectory ?? Environment.CurrentDirectory;
#if OS_WINDOWS
#pragma warning disable CA1416 // Validate platform compatibility
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

        if (Command.AdminRequired && !principal.IsInRole(WindowsBuiltInRole.Administrator))
            return new ProcessStartInfo
            {
                WorkingDirectory = dir,
                FileName = "cmd.exe",
                Arguments = $"/c \"{cmd}\"",
                Verb = "runas",
                UseShellExecute = true
            };
        return new ProcessStartInfo
        {
            WorkingDirectory = dir,
            FileName = "cmd.exe",
            Arguments = $"/c \"{cmd}\""
        };
#pragma warning restore CA1416 // Validate platform compatibility

#elif OS_LINUX
        if (!Command.AdminRequired)
            return new ProcessStartInfo
            {
                WorkingDirectory = dir,
                FileName = "/bin/bash",
                Arguments = $"-c \"{cmd}\"",
            };
        return new ProcessStartInfo
        {
            FileName = "/bin/sudo",
            Arguments = $"/bin/bash -c \"cd {dir} && {cmd}\""
        };
#elif OS_MAC
        if (!Command.AdminRequired)
            return new ProcessStartInfo
            {
                WorkingDirectory = Command.WorkingDirectory ?? Environment.CurrentDirectory,
                FileName = "/bin/bash",
                Arguments = $"-c \"{cmd}\"",
            };
        return new ProcessStartInfo
        {
            WorkingDirectory = Command.WorkingDirectory ?? Environment.CurrentDirectory,
            FileName = "/bin/bash",
            Arguments = $"-c \"sudo {cmd}\"",
        };
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
using System.Diagnostics;
using Cmaner.Holder;
using System.Security.Principal;

namespace Cmaner;

public class Runner
{
    private Command Command { get; }


    private ProcessStartInfo GetProcessStartInfo(string[] lArg)
    {
#if OS_WINDOWS
        return GetWindowsStartInfo(lArg);
#elif OS_LINUX
        return GetLinuxStartInfo(lArg);
#else
    #error Unsupported OS
#endif
    }

    private ProcessStartInfo GetWindowsStartInfo(IEnumerable<string> lArg)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        const string executor = "cmd.exe";
        var lArgs = string.Join(" ", lArg.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
        var dir = Command.WorkingDirectory ?? Environment.CurrentDirectory;
        var cmd = Command.CommandText.Replace("\"", "\"");
        var cmdArgs = $"/c {cmd} {lArgs}";
        
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        if (Command.AdminRequired && !principal.IsInRole(WindowsBuiltInRole.Administrator))
            return new ProcessStartInfo
            {
                WorkingDirectory = dir,
                FileName = executor,
                Arguments = $"{cmdArgs} && pause",
                Verb = "runas",
                UseShellExecute = true
            };
        return new ProcessStartInfo
        {
            WorkingDirectory = dir,
            FileName = executor,
            Arguments = cmdArgs,
        };
#pragma warning restore CA1416 // Validate platform compatibility
    }
    private ProcessStartInfo GetLinuxStartInfo(IEnumerable<string> lArg)
    {
        var lArgs = string.Join(" ", lArg.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));
        var dir = Command.WorkingDirectory ?? Environment.CurrentDirectory;
        var cmd = Command.CommandText.Replace("\"", "\\\"");
        lArgs = lArgs.Replace("\"", "\\\"");
        if (!Command.AdminRequired)
            return new ProcessStartInfo
            {
                WorkingDirectory = dir,
                FileName = "/bin/bash",
                Arguments = $"-c \"{cmd} {lArgs}\"",
            };
        return new ProcessStartInfo
        {
            FileName = "/bin/sudo",
            Arguments = $"/bin/bash -c \"cd {dir} && {cmd} {lArgs}\"",
        };
    }
    public Runner(Command command) => Command = command;

    public async Task RunAsync(string[] lArg)
    {
        var process = new Process
        {
            StartInfo = GetProcessStartInfo(lArg)
        };
        process.Start();
        await process.WaitForExitAsync();
    }
}
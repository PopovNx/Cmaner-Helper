using System.Diagnostics;
using Cmaner.Holder;
using System.Security.Principal;

namespace Cmaner;

public class Runner
{
    private Command Command { get; }

    private string WorkDirectory =>
        (Command.Flags.HasFlag(CmdFlags.HasWorkingDirectory)
            ? Command.WorkingDirectory
            : Environment.CurrentDirectory)
        ?? Environment.CurrentDirectory;


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

    private ProcessStartInfo GetWindowsStartInfo(IReadOnlyCollection<string> lArg)
    {
#pragma warning disable CA1416 // Validate platform compatibility
        const string executor = "cmd.exe";
        var lArgs = string.Join(" ", lArg.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));

        var cmd = Command.CommandText;

        var cmdArgs = cmd.Contains("%args") ? $" {cmd.Replace("%args", lArgs)}" : $"{cmd} {lArgs}";
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        if (Command.Flags.HasFlag(CmdFlags.AdminRequired) && !principal.IsInRole(WindowsBuiltInRole.Administrator))
            return new ProcessStartInfo
            {
                FileName = executor,
                Arguments = $"/c cd /d \"{WorkDirectory}\" & {cmdArgs} & pause",
                Verb = "runas",
                UseShellExecute = true
            };
        return new ProcessStartInfo
        {
            WorkingDirectory = WorkDirectory,
            FileName = executor,
            Arguments = $"/c {cmdArgs}",
        };
#pragma warning restore CA1416 // Validate platform compatibility
    }

    private ProcessStartInfo GetLinuxStartInfo(IReadOnlyCollection<string> lArg)
    {
        var lArgs = string.Join(" ", lArg.Select(a => a.Contains(' ') ? $"\"{a}\"" : a));


        var cmd = Command.CommandText.Replace("\"", "\\\"");
        lArgs = lArgs.Replace("\"", "\\\"");

        string cmdArgs;
        if (cmd.Contains("%args"))
        {
            cmdArgs = $"{cmd.Replace("%args", lArgs)}";
        }
        else if (lArg.Count > 0)
        {
            cmdArgs = $"{cmd} {lArgs}";
        }
        else
        {
            cmdArgs = $"{cmd}";
        }

        if (!Command.Flags.HasFlag(CmdFlags.AdminRequired))
            return new ProcessStartInfo
            {
                WorkingDirectory = WorkDirectory,
                FileName = "/bin/bash",
                Arguments = $"-c \"{cmdArgs}\"",
            };
        
        return new ProcessStartInfo
        {
            FileName = "/bin/sudo",
            Arguments = $"/bin/bash -c \"cd {WorkDirectory} && {cmdArgs}\""
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
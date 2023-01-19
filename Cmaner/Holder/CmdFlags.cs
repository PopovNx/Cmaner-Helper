namespace Cmaner.Holder;

[Flags]
public enum CmdFlags
{
    None = 0,
    HasTitle = 1,
    HasDescription = 2,
    HasShortCall = 4,
    HasWorkingDirectory = 8,
    AdminRequired = 16,
    SilentExecution = 32,
    RequestArguments = 64,
    RequestConfirmation = 128,
    HideCommandText = 256,
    Highlight = 512,
}
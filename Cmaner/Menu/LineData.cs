namespace Cmaner.Menu;

public class LineData
{
    public required string Text { get; init; }

    public ConsoleColor? ForegroundColor { get; init; }
    public ConsoleColor? BackgroundColor { get; init; }

    public static implicit operator LineData(string text) => new() { Text = text };
    public override string ToString() => Text;
}

internal static class LineDataExtensions
{
    public static LineData Color(this string line, ConsoleColor color) =>
        new() { Text = line, ForegroundColor = color };

    public static LineData Background(this string line, ConsoleColor color) =>
        new() { Text = line, BackgroundColor = color };

    public static LineData Color(this LineData line, ConsoleColor color) =>
        new() { Text = line.Text, ForegroundColor = color };

    public static LineData Background(this LineData line, ConsoleColor color) =>
        new() { Text = line.Text, BackgroundColor = color };
}
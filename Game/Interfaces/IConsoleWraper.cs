namespace BetGame.Interfaces
{
    public interface IConsoleWrapper
    {
        void WriteLine(string message);
        string? ReadLine();
        void SetTitle(string title);
        void SetForegroundColor(ConsoleColor color);
        void ResetColor();
        ConsoleColor GetColor(string color);
    }
}
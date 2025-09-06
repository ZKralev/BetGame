using BetGame.Interfaces;

namespace BetGame.Abstractions
{
    public class ConsoleWrapper : IConsoleWrapper
    {
        public void WriteLine(string message) => Console.WriteLine(message);
        public string? ReadLine() => Console.ReadLine();

        public void SetTitle(string title) => Console.Title = title;

        public void SetForegroundColor(ConsoleColor color) => Console.ForegroundColor = color;

        public void ResetColor() => Console.ResetColor();

        public static void ConsoleReadLine() => Console.ReadLine();
    }
}

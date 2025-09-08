namespace BetGame.Abstractions
{
    public class BetResult(string message, double? winAmount, ConsoleColor color)
    {
        public string Message { get; } = message;
        public double? WinAmount { get; } = winAmount;
        public ConsoleColor Color { get; } = color;
    }
}

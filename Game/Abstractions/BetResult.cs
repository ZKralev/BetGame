namespace BetGame.Abstractions
{
    public class BetResult
    {
        public double Roll { get; set; }
        public string Message { get; set; }

        public double? WinAmount { get; set; }
        public ConsoleColor Color { get; set; }

        public BetResult(double roll, string message, double winAmount, ConsoleColor color)
        {
            Roll = Roll;
            Message = message;
            WinAmount = winAmount;
            Color = color;
        }

    }
}

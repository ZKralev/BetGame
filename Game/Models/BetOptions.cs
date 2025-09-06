namespace BetGame.Models;

public class BetOptions
{
    public const string SectionName = "BetOptions";
    
    public double SmallestBet { get; set; }
    public double BiggestBet { get; set; }
    public double MinimalRoll { get; set; }
    public double MaxRoll { get; set; } 
    public SmallWinBetOutcome SmallWin { get; set; } = null!;
    public BigWinBetOutcome BigWin { get; set; } = null!;
}

public class SmallWinBetOutcome 
{
    public double WinRatioStart { get; set; }
    public double WinRatioEnd { get; set; }
}

public class BigWinBetOutcome 
{
    public double WinRatioStart { get; set; }
    public double WinRatioEnd { get; set; }
}

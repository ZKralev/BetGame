using BetGame.Interfaces;
using BetGame.Abstractions;
using BetGame.Models;
using Microsoft.Extensions.Options;

namespace BetGame.Services;

public class BetEngineService(IOptions<BetOptions> options, IRandomGenerator rnd, IConsoleWrapper console) : IBetEngine
{
    private readonly IConsoleWrapper _console = console;
    private readonly IRandomGenerator _randomGenerator = rnd;
    private readonly BetOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

    public BetOptions GetBetOptions() => _options;
    public IRandomGenerator? GetRandomGenerator() => _randomGenerator;

    public BetResult PlayRound(double stake)
    {
        var opt = GetBetOptions() ?? throw new InvalidOperationException("Bet options are not configured.");

        if (stake < opt.SmallestBet || stake > opt.BiggestBet)
            throw new ArgumentOutOfRangeException($"Bet must be between {opt.SmallestBet} and {opt.BiggestBet}.");

        double winAmount;
        double jackpotMult;
        double roll = GetRandomMultiplier(opt.MinimalRoll, opt.MaxRoll);

        if (roll <= 0.5)
            return new BetResult(roll, "No luck this time! Your current balance is: $", 0, ConsoleColor.Red);

        if (roll <= 0.9)
        {
            jackpotMult = GetRandomMultiplier(opt.SmallWin.WinRatioStart, opt.SmallWin.WinRatioEnd);
            winAmount = stake * jackpotMult;
            return new BetResult(roll, $"Congratulations! You win ${winAmount:F2}! Your current balance is: $", winAmount, ConsoleColor.Green);
        }

        jackpotMult = GetRandomMultiplier(opt.BigWin.WinRatioStart, opt.BigWin.WinRatioEnd);
        winAmount = stake * jackpotMult;
        return new BetResult(roll, $"JACKPOT! You win ${winAmount:F2}! Your current balance is: $", winAmount, ConsoleColor.Green);
    }

    private double GetRandomMultiplier(double min, double max)
    {
        var rnd = GetRandomGenerator() ?? throw new InvalidOperationException("Random generator is not initialized.");
        var multiplier = min + max * rnd.NextDouble();
        return (double)multiplier;
    }
}
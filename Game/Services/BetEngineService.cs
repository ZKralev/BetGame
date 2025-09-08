using BetGame.Interfaces;
using BetGame.Abstractions;
using BetGame.Models;
using Microsoft.Extensions.Options;

namespace BetGame.Services;

public class BetEngineService(IOptions<BetOptions> options, IRandomGenerator random, IConsoleWrapper console) : IBetEngine
{

    public BetResult PlayRound(double stake)
    {

        if (stake < options.Value.SmallestBet || stake > options.Value.BiggestBet)
            throw new ArgumentOutOfRangeException($"Bet must be between {options.Value.SmallestBet} and {options.Value.BiggestBet}.");

        double winAmount;
        double jackpotMult;
        double roll = GetRandomMultiplier(options.Value.MinimalRoll, options.Value.MaxRoll);

        if (roll <= 0.5)
            return new BetResult("No luck this time! Your current balance is: $", 0, console.GetColor("Red"));

        if (roll <= 0.9)
        {
            jackpotMult = GetRandomMultiplier(options.Value.SmallWin.WinRatioStart, options.Value.SmallWin.WinRatioEnd);
            winAmount = stake * jackpotMult;
            return new BetResult($"Congratulations! You win ${winAmount:F2}! Your current balance is: $", winAmount, console.GetColor("Green"));
        }

        jackpotMult = GetRandomMultiplier(options.Value.BigWin.WinRatioStart, options.Value.BigWin.WinRatioEnd);
        winAmount = stake * jackpotMult;
        return new BetResult($"JACKPOT! You win ${winAmount:F2}! Your current balance is: $", winAmount, console.GetColor("Green"));
    }

    private double GetRandomMultiplier(double min, double max)
    {
        var rnd = random ?? throw new InvalidOperationException("Random generator is not initialized.");
        var multiplier = min + max * rnd.NextDouble();
        return multiplier;
    }
}
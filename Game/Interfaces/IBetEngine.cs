using BetGame.Abstractions;
using BetGame.Models;

namespace BetGame.Interfaces
{
    public interface IBetEngine
    {
        BetResult PlayRound(double stake);
        BetOptions GetBetOptions();
    }
}

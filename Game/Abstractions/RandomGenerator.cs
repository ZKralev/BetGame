using BetGame.Interfaces;

namespace BetGame.Abstractions
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly Random random = new();
        public double NextDouble() => random.NextDouble();
    }
}

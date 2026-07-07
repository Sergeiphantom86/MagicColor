using System.Collections.Generic;

namespace PuzzleResources.MinigamesRoulette
{
    public class WeightCalculator
    {
        private const int HighProbability = 100;
        private const int MediumProbability = 80;
        private const int ReducedProbability = 60;
        private const int LowProbability = 40;
        private const int MinimalProbability = 20;

        private readonly Dictionary<int, int> _weightMap = new()
        {
            [1] = HighProbability,
            [2] = HighProbability,
            [3] = MediumProbability,
            [50] = MediumProbability,
            [4] = ReducedProbability,
            [100] = ReducedProbability,
            [150] = LowProbability,
            };

            public int GetWeight(int value)
            {
                return _weightMap.TryGetValue(value, out int weight) ? weight : MinimalProbability;
            }
        }
    }
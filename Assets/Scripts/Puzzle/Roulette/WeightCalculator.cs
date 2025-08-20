public class WeightCalculator
{
    private const int HighProbability = 100;
    private const int MediumProbability = 80;
    private const int ReducedProbability = 60;
    private const int LowProbability = 40;
    private const int MinimalProbability = 20;

    private const int Coin1 = 1;
    private const int Coin2 = 2;
    private const int Coin3 = 3;
    private const int Coin4 = 4;
    private const int Crystal50 = 50;
    private const int Crystal100 = 100;
    private const int Crystal150 = 150;

    public int GetWeight(int value)
    {
        return value switch
        {
            Coin1 or Coin2 => HighProbability,
            Coin3 or Crystal50 => MediumProbability,
            Coin4 or Crystal100 => ReducedProbability,
            Crystal150 => LowProbability,
            _ => MinimalProbability
        };
    }
}
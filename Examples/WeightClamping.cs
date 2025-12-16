namespace Randomness.Distributing;

public struct WeightClamping : IWeightDistributionPolicy
{
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }

    public WeightClamping()
    {
        LowerBound = 0;
        UpperBound = 1;
    }

    public WeightClamping(double lowerBound, double upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }
    public readonly void ApplyTo(Span<double> weights)
    {
        double lower = LowerBound;
        double upper = UpperBound;
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Math.Clamp(weights[i], lower, upper);
        }
    }
}
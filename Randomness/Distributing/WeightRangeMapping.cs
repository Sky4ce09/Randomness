namespace Randomness.Distributing;

/// <summary>
/// An exemplary <see cref="IWeightDistributionPolicy"/> implementer that linearly maps generated weights onto the range [<see cref="LowerBound"/>, <see cref="UpperBound"/>).
/// </summary>
public struct WeightRangeMapping : IWeightDistributionPolicy
{
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }

    /// <summary>
    /// Initializes a <see cref="WeightRangeMapping"/> with the range [0, 1).
    /// </summary>
    public WeightRangeMapping()
    {
        LowerBound = 0;
        UpperBound = 1;
    }

    /// <summary>
    /// Initializes a <see cref="WeightRangeMapping"/> with the range [<paramref name="lowerBound"/>, <paramref name="upperBound"/>).
    /// </summary>
    public WeightRangeMapping(double lowerBound, double upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }
    public readonly void ApplyTo(Span<double> weights)
    {
        double lowerBound = LowerBound;
        double spanning = UpperBound - lowerBound;
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = lowerBound + (weights[i] * spanning);
        }
    }
}
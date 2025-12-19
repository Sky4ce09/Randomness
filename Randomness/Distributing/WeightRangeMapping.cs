using System.Numerics;

namespace Randomness.Distributing;

/// <summary>
/// An exemplary <see cref="IWeightDistributionPolicy"/> implementer that linearly maps weights from a source range to a target range. Works with negative weights.
/// </summary>
public struct WeightRangeMapping : IWeightDistributionPolicy
{
    public double ToLowerBound { get; set; }
    public double ToUpperBound { get; set; }
    public double FromLowerBound { get; set; }
    public double FromUpperBound { get; set; }

    /// <summary>
    /// Initializes a <see cref="WeightRangeMapping"/> with the source and target ranges [0, 1).
    /// </summary>
    public WeightRangeMapping()
    {
        FromLowerBound = 0;
        FromUpperBound = 1;
        ToLowerBound = 0;
        ToUpperBound = 1;
    }

    /// <summary>
    /// Initializes a <see cref="WeightRangeMapping"/> with the resulting range [<paramref name="toLowerBound"/>, <paramref name="toUpperBound"/>), assuming that weights processed by this mapping fall within [0, 1).
    /// </summary>
    /// <param name="toLowerBound">The lower boundary of the range which weights get mapped to.</param>
    /// <param name="toUpperBound">The upper boundary of the range which weights get mapped to.</param>
    public WeightRangeMapping(double toLowerBound, double toUpperBound)
    {
        FromLowerBound = 0;
        FromUpperBound = 1;
        ToLowerBound = toLowerBound;
        ToUpperBound = toUpperBound;
    }

    /// <summary>
    /// Initializes a <see cref="WeightRangeMapping"/> with the resulting range [<paramref name="toLowerBound"/>, <paramref name="toUpperBound"/>), assuming that the source range [<paramref name="fromLowerBound"/>, <paramref name="fromUpperBound"/>) is correct.
    /// </summary>
    /// <param name="fromLowerBound">The lower boundary of the range which weights are assumed to be in.</param>
    /// <param name="fromUpperBound">The upper boundary of the range which weights are assumed to be in.</param>
    /// <param name="toLowerBound">The lower boundary of the range which weights get mapped to.</param>
    /// <param name="toUpperBound">The upper boundary of the range which weights get mapped to.</param>
    public WeightRangeMapping(double fromLowerBound, double fromUpperBound, double toLowerBound, double toUpperBound)
    {
        ToLowerBound = toLowerBound;
        ToUpperBound = toUpperBound;
        FromLowerBound = fromLowerBound;
        FromUpperBound = fromUpperBound;
    }
    public readonly void ApplyTo(Span<double> weights)
    {
        double fromLowerBound = FromLowerBound;
        double fromSpanning = FromUpperBound - fromLowerBound;
        double toLowerBound = ToLowerBound;

        if (fromSpanning != 0)
        {
            double toSpanning = ToUpperBound - toLowerBound;
            double scale = toSpanning / fromSpanning;

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = toLowerBound + ((weights[i] - fromLowerBound) * scale);
            }
        }
        else
        {
            double midpoint = (toLowerBound + ToUpperBound) * 0.5;
            weights.Fill(midpoint);
        }
    }
}
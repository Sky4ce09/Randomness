namespace Randomness.Sampling;

/// <summary>
/// A wrapper for .NET's System.<see cref="System.Random"/> that generates white (random) noise values.
/// </summary>
public class WhiteNoise : ISampler
{
    public Random Random;
    public WhiteNoise(Random random)
    {
        Random = random;
    }
    public WhiteNoise() : this(new Random()) { }
    public double? AddSample(Span<double> outValues)
    {
        for (int i = 0; i < outValues.Length; i++)
        {
            outValues[i] += Random.NextDouble();
        }
        return null;
    }
}

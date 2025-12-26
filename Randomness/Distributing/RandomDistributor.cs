using Randomness.Sampling;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

/// <summary>
/// A randomizer / distributor that distributes a number value across multiple elements in a span using weighted distribution. <para/>
/// This builds on top of .NET's System.<see cref="System.Random"/> and utilizes it to generate statistically random weight distributions. <para/>
/// Generates weights in the range [0, 1) by default.
/// </summary>
/// <remarks>
/// None of this class's public methods are thread-safe.
/// </remarks>
public sealed class RandomDistributor : ConfigurableDistributor
{
    // this thing is skill issue api baggage
    /// <summary>
    /// The System.<see cref="System.Random"/> utilized in the weight generation algorithm.
    /// </summary>
    public Random Random {
        get { return _random; }
        set { Sampler.Random = value; _random = value; }
    }
    private Random _random;
    private new WhiteNoise Sampler { get; init; }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a provided System.<see cref="System.Random"/> object.
    /// </summary>
    /// <param name="random">The System.<see cref="System.Random"/> object to be provided.</param>
    public RandomDistributor(Random random)
    {
        _random = random;
        Sampler = new(random);
        Distributor = new ValueDistributor();
    }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a new System.<see cref="System.Random"/> instance.
    /// </summary>
    public RandomDistributor() : this(new Random()) { } 

    [Obsolete("Generic alternative has been introduced.")]
    public void Distribute(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.Distribute(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public int[] Distribute(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        int[] target = new int[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    [Obsolete("Generic alternative has been introduced.")]
    public void DistributeApproximate(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.DistributeApproximate(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public int[] DistributeApproximate(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        int[] target = new int[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    [Obsolete("Generic alternative has been introduced.")]
    public void Distribute(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.Distribute(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public long[] Distribute(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    [Obsolete("Generic alternative has been introduced.")]
    public void DistributeApproximate(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.DistributeApproximate(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public long[] DistributeApproximate(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    [Obsolete("Generic alternative has been introduced.")]
    public void Distribute(double value, Span<double> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.Distribute(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public double[] Distribute(double value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        double[] target = new double[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    [Obsolete("Generic alternative has been introduced.")]
    public void Distribute(decimal value, Span<decimal> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers);

        Distributor.Distribute(value, weights, target);
    }

    [Obsolete("Generic alternative has been introduced.")]
    public decimal[] Distribute(decimal value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        decimal[] target = new decimal[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }
    private void GenerateWeights(Span<double> weights, IWeightDistributionPolicy[] weightModifiers)
    {
        for (int i = 0; i < weights.Length; i++) weights[i] = Random.NextDouble();
        foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);
    }
}

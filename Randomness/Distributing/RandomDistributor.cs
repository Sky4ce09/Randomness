using System.Numerics;

namespace Randomness.Distributing;

/// <summary>
/// Represents a randomizer that distributes a number value across multiple elements in a span using weighted distribution. <para/>
/// This builds on top of .NET's System.<see cref="System.Random"/> and utilizes it to generate statistically random weight distributions.
/// </summary>
/// <remarks>
/// None of this class's public methods are thread-safe.
/// </remarks>
public sealed class RandomDistributor
{
    /// <summary>
    /// The System.<see cref="System.Random"/> utilized in the weight generation algorithm.
    /// </summary>
    public Random Random { get; set; }
    private UncheckedDistributor Distributor { get; init; }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a provided System.<see cref="System.Random"/> object.
    /// </summary>
    /// <param name="random">The System.<see cref="System.Random"/> object to be provided.</param>
    public RandomDistributor(Random random)
    {
        Random = random;
        Distributor = new UncheckedDistributor();
    }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a new System.<see cref="System.Random"/> instance.
    /// </summary>
    public RandomDistributor() : this(new Random()) { }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Distribute(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.Distribute(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across a new array of length <paramref name="spread"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int[] Distribute(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        int[] target = new int[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void DistributeApproximate(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.DistributeApproximate(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across  a new array of length <paramref name="spread"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public int[] DistributeApproximate(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        int[] target = new int[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a native integer <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Distribute(nint value, Span<nint> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.Distribute(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a native integer <paramref name="value"/> across a new array of length <paramref name="spread"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public nint[] Distribute(nint value, nint spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        nint[] target = new nint[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a native integer <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void DistributeApproximate(nint value, Span<nint> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.DistributeApproximate(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a native integer <paramref name="value"/> across a new array of length <paramref name="spread"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public nint[] DistributeApproximate(nint value, nint spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        nint[] target = new nint[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Distribute(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.Distribute(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across a new array of length <paramref name="spread"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public long[] Distribute(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void DistributeApproximate(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.DistributeApproximate(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across a new array of length <paramref name="spread"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public long[] DistributeApproximate(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a double <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Distribute(double value, Span<double> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.Distribute(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a double <paramref name="value"/> across a new array of length <paramref name="spread"/>. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public double[] Distribute(double value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        double[] target = new double[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a decimal <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Distribute(decimal value, Span<decimal> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        Distributor.Distribute(value, weights, weightSum, target);
    }

    /// <summary>
    /// Distributes a decimal <paramref name="value"/> across a new array of length <paramref name="spread"/>. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public decimal[] Distribute(decimal value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        decimal[] target = new decimal[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }
    private void GenerateWeights(Span<double> weights, IWeightDistributionPolicy[] weightModifiers, out double weightSum)
    {
        weightSum = 0;
        for (int i = 0; i < weights.Length; i++) weights[i] = Random.NextDouble();
        foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);

        double largestAbsWeight = double.NegativeInfinity;
        Span<bool> signsEncountered = stackalloc bool[] { false, false }; // positive, negative

        for (int i = 0; i < weights.Length; i++)
        {
            double weight = weights[i];
            if (double.IsNaN(weight))
            {
                System.Diagnostics.Debug.Fail("Weight distribution policies produced NaN value.");
                weight = 0;
                weights[i] = weight;
            }
            else if (weight == double.PositiveInfinity || weight == double.NegativeInfinity)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "Weight distribution policies produced reserved infinity value.");
            }
            else
            {
                double absWeight = Math.Abs(weight);
                if (absWeight > largestAbsWeight) largestAbsWeight = absWeight;
            }
            signsEncountered[weight < 0 ? 1 : 0] = true;
            weightSum += weight;
        }
        if (weightSum == 0)
        {
            if ((signsEncountered[0], signsEncountered[1]) != (true, true))
            {
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = 1;
                }
                weightSum = weights.Length;
            }
            else
            {
                weightSum = Math.Max(double.Epsilon * 2.1D, double.Epsilon * largestAbsWeight * 2.1D); // i hope this prevents infinities
            }
        }
        if (weightSum == 0) throw new DivideByZeroException("Could not generate a valid weight distribution."); // -0 weight party tricks
    }
}

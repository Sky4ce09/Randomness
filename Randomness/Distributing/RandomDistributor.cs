using System.Linq;
using System.Numerics;

namespace Randomness.Distributing;

/// <summary>
/// Represents a randomizer that distributes a number value across multiple elements of an array using weighted distribution. <para/>
/// This builds on top of .NET's System.<see cref="System.Random"/> and utilizes it to generate statistically random weight distributions.
/// </summary>
/// <remarks>
/// None of this class's public methods are thread-safe.
/// </remarks>
public class RandomDistributor
{
    private static int s_maxWeightGenerationAttemptCount = 256;
    /// <summary>
    /// The System.<see cref="System.Random"/> utilized by the <see cref="RandomDistributor"/> object.
    /// </summary>
    public Random Random { get; set; }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a provided System.<see cref="System.Random"/> object.
    /// </summary>
    /// <param name="random">The System.<see cref="System.Random"/> object to be provided.</param>
    public RandomDistributor(Random random)
    {
        Random = random;
    }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> with a new System.<see cref="System.Random"/> instance.
    /// </summary>
    public RandomDistributor() : this(new Random()) { }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        Span<int> distributed = target.Length <= 256 ? stackalloc int[target.Length] : new int[target.Length];
        double doubleValue = Convert.ToDouble(value);
        int totalDistributedValue = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            int distribution = (int)(doubleValue * weights[i] / weightSum);
            totalDistributedValue += distribution;
            distributed[i] = distribution;
        }

        // handle discrepancy between value and distributed
        int remainingValue = value - totalDistributedValue;
        if (remainingValue != 0) DistributeRemainder(target, distributed, weights, remainingValue);

        // apply pre-remainder distribution
        for (int i = 0; i < target.Length; i++)
        {
            target[i] += distributed[i];
        }
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(int value, int[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<int>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across a new array of length <paramref name="spread"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception
    /// <exception cref="DivideByZeroException"></exception>
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
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
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
    /// <exception cref="DivideByZeroException"></exception>
    public void DistributeApproximate(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        double doubleValue = Convert.ToDouble(value);
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += (int)Math.Round(doubleValue * weights[i] / weightSum);
        }
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void DistributeApproximate(int value, int[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        DistributeApproximate(value, (Span<int>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes an integer <paramref name="value"/> across  a new array of length <paramref name="spread"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
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
    /// <exception cref="DivideByZeroException"></exception>
    public int[] DistributeApproximate(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        if (spread < 1) throw new ArgumentOutOfRangeException(nameof(spread));
        int[] target = new int[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        Span<long> distributed = target.Length <= 256 ? stackalloc long[target.Length] : new long[target.Length];
        double doubleValue = Convert.ToDouble(value);
        long totalDistributedValue = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            long distribution = (long)(doubleValue * weights[i] / weightSum);
            totalDistributedValue += distribution;
            distributed[i] = distribution;
        }

        // handle discrepancy between value and distributed
        int remainingValue = (int)(value - totalDistributedValue); // guaranteed remainingValue <= target.Length, lossless cast
        if (remainingValue != 0) DistributeRemainder(target, distributed, weights, remainingValue);

        // apply pre-remainder distribution
        for (int i = 0; i < target.Length; i++)
        {
            target[i] += distributed[i];
        }
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(long value, long[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<long>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across a new array of length <paramref name="spread"/> precisely, leaving no remainder. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public long[] Distribute(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
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
    /// <exception cref="DivideByZeroException"></exception>
    public void DistributeApproximate(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        double doubleValue = Convert.ToDouble(value);
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += (long)Math.Round(doubleValue * weights[i] / weightSum);
        }
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>.
    /// Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void DistributeApproximate(long value, long[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        DistributeApproximate(value, (Span<long>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes a long <paramref name="value"/> across a new array of length <paramref name="spread"/> using weighted proportions and per-element rounding.
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
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
    /// <exception cref="DivideByZeroException"></exception>
    public long[] DistributeApproximate(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        DistributeApproximate(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a double <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(double value, Span<double> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        double distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            double share = value * weights[i] / weightSum;
            target[i] += share;
            distributed += share;
        }

        // mitigate rounding errors
        target[Random.Next(0, target.Length)] += value - distributed;
    }

    /// <summary>
    /// Distributes a double <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(double value, double[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<double>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes a double <paramref name="value"/> across a new array of length <paramref name="spread"/>. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public double[] Distribute(double value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        double[] target = new double[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes a decimal <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target memory span.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(decimal value, Span<decimal> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        decimal distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            decimal share = value * (decimal)weights[i] / (decimal)weightSum;
            target[i] += share;
            distributed += share;
        }

        // mitigate rounding errors
        target[Random.Next(0, target.Length)] += value - distributed;
    }

    /// <summary>
    /// Distributes a decimal <paramref name="value"/> across <paramref name="target"/>. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target array is empty. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="target">The target array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public void Distribute(decimal value, decimal[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<decimal>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes a decimal <paramref name="value"/> across a new array of length <paramref name="spread"/>. <para/>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="spread"/> is less than 1. <para/>
    /// Throws a <see cref="DivideByZeroException"/> if the weight generation algorithm is unable to generate valid weights. This never happens when all weights share the same sign.
    /// </summary>
    /// <param name="value">The value to be distributed.</param>
    /// <param name="spread">The length of the new array.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="DivideByZeroException"></exception>
    public decimal[] Distribute(decimal value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        decimal[] target = new decimal[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }
    private void GenerateWeights(Span<double> weights, IWeightDistributionPolicy[] weightModifiers, out double weightSum)
    {
        weightSum = 0;
        for (int attempts = 0; attempts < s_maxWeightGenerationAttemptCount; attempts++)
        {
            for (int i = 0; i < weights.Length; i++) weights[i] = Random.NextDouble();
            foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);

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
                signsEncountered[Convert.ToInt32(weight < 0)] = true;
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
                    break;
                } else
                {
                    continue;
                }
            }
            break;
        }
        if (weightSum == 0) throw new DivideByZeroException("Could not generate a valid weight distribution.");
    }
    private void DistributeRemainder<T>(Span<T> targetValues, Span<T> distributedValues, Span<double> weights, int remainder) where T : INumber<T>
    {
        // remainder originates solely from truncation, so each index should only receive at most one additional incrementation

        Span<double> demands = weights.Length <= 256 ? stackalloc double[weights.Length] : new double[weights.Length];
        for (int i = 0; i < demands.Length; i++) demands[i] = weights[i] / (double.CreateSaturating(distributedValues[i]) + 1);
        if (remainder > 0)
        {
            PriorityQueue<int, double> highestDemandIndices = new(remainder);
            for (int i = 0; i < demands.Length; i++)
            {
                double demand = demands[i];
                if (highestDemandIndices.Count < remainder)
                {
                    highestDemandIndices.Enqueue(i, demand);
                }
                else
                {
                    highestDemandIndices.TryPeek(out _, out double lowestQueuedDemand);
                    if (demand <= lowestQueuedDemand) continue;
                    highestDemandIndices.Dequeue();
                    highestDemandIndices.Enqueue(i, demand);
                }
            }
            while (highestDemandIndices.Count > 0) targetValues[highestDemandIndices.Dequeue()]++;
        }
        else
        {
            remainder *= -1;
            // handle negative remainder created by negative weights
            PriorityQueue<int, double> lowestDemandIndices = new(remainder, Comparer<double>.Create((a, b) => b.CompareTo(a)));
            for (int i = 0; i < demands.Length; i++)
            {
                double demand = demands[i];
                if (lowestDemandIndices.Count < remainder)
                {
                    lowestDemandIndices.Enqueue(i, demand);
                }
                else
                {
                    lowestDemandIndices.TryPeek(out _, out double lowestQueuedDemand);
                    if (demand >= lowestQueuedDemand) continue;
                    lowestDemandIndices.Dequeue();
                    lowestDemandIndices.Enqueue(i, demand);
                }
            }
            while (lowestDemandIndices.Count > 0) targetValues[lowestDemandIndices.Dequeue()]--;
        }
    }
}

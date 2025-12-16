using System.Numerics;

namespace Randomness.Distributing;

/// <summary>
/// Represents a randomizer that distributes a number value across multiple elements of an array using weighted distribution. <para/>
/// This builds on top of .NET's System.<see cref="System.Random"/> class and utilizes it to generate statistically random weight distributions.
/// </summary>
public class RandomDistributor
{
    /// <summary>
    /// The System.<see cref="System.Random"/> object utilized by the <see cref="RandomDistributor"/> object.
    /// </summary>
    public Random Random { get; set; }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> object with a provided System.<see cref="System.Random"/> object.
    /// </summary>
    /// <param name="random">The System.<see cref="System.Random"/> object to be provided.</param>
    public RandomDistributor(Random random)
    {
        Random = random;
    }

    /// <summary>
    /// Initializes a new <see cref="RandomDistributor"/> object with a new System.<see cref="System.Random"/> instance.
    /// </summary>
    public RandomDistributor() : this(new Random()) { }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across the <paramref name="target"/> array randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers. <para/>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="target"/> has no elements.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="target">The array to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the target array has no elements.
    /// </exception>
    /// <remarks>
    /// This method mutates <paramref name="target"/> in place.
    /// </remarks>
    public void Distribute(int value, Span<int> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target array must have a length of at least one.");

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

    public void Distribute(int value, int[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<int>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across a new array of length <paramref name="spread"/> randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="spread">The amount of number elements to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    public int[] Distribute(int value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        int[] target = new int[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across the <paramref name="target"/> array randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers. <para/>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="target"/> has no elements.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="target">The array to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the target array has no elements.
    /// </exception>
    /// <remarks>
    /// This method mutates <paramref name="target"/> in place.
    /// </remarks>
    public void Distribute(long value, Span<long> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target array must have a length of at least one.");

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

    public void Distribute(long value, long[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<long>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across a new array of length <paramref name="spread"/> randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="spread">The amount of number elements to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    public long[] Distribute(long value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        long[] target = new long[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across the <paramref name="target"/> array randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers.
    /// Throws an <see cref="ArgumentException"/> if <paramref name="target"/> has no elements.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="target">The array to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the target array has no elements.
    /// </exception>
    /// <remarks>
    /// This method mutates <paramref name="target"/> in place.
    /// </remarks>
    public void Distribute(double value, Span<double> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target array must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += value * weights[i] / weightSum;
        }
    }

    public void Distribute(double value, double[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<double>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across a new array of length <paramref name="spread"/> randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="spread">The amount of number elements to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    public double[] Distribute(double value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        double[] target = new double[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across the <paramref name="target"/> array randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers. <para/>
    /// Throws an <see cref="ArgumentException"/> if <paramref name="target"/> has no elements.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="target">The array to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the target array has no elements.
    /// </exception>
    /// <remarks>
    /// This method mutates <paramref name="target"/> in place.
    /// </remarks>
    public void Distribute(decimal value, Span<decimal> target, params IWeightDistributionPolicy[] weightModifiers)
    {
        // reject elementless arrays
        if (target.Length < 1) throw new ArgumentException("Target array must have a length of at least one.");

        // generate random weights
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        GenerateWeights(weights, weightModifiers, out double weightSum);

        // distribute value
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += value * (decimal)weights[i] / (decimal)weightSum;
        }
    }

    public void Distribute(decimal value, decimal[] target, params IWeightDistributionPolicy[] weightModifiers)
    {
        Distribute(value, (Span<decimal>)target, weightModifiers);
    }

    /// <summary>
    /// Distributes the specified <paramref name="value"/> across a new array of length <paramref name="spread"/> randomly. <para/>
    /// The randomness is optionally tuned by any provided <see cref="IWeightDistributionPolicy"/> implementers.
    /// </summary>
    /// <param name="value">The amount of units to be distributed.</param>
    /// <param name="spread">The amount of number elements to receive the distributed value.</param>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be used. (optional)</param>
    public decimal[] Distribute(decimal value, int spread, params IWeightDistributionPolicy[] weightModifiers)
    {
        decimal[] target = new decimal[spread];
        Distribute(value, target, weightModifiers);
        return target;
    }
    private void GenerateWeights(Span<double> weights, IWeightDistributionPolicy[] weightModifiers, out double weightSum)
    {
        for (int i = 0; i < weights.Length; i++) weights[i] = Random.NextDouble();
        foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);

        weightSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            double weight = Math.Max(0, weights[i]);
            if (double.IsNaN(weight))
            {
                System.Diagnostics.Debug.Fail("Weight distribution policies produced NaN value.");
                weight = 0;
            }
            else if (weight == double.PositiveInfinity)
            {
                throw new ArgumentOutOfRangeException(nameof(weights), "Weight distribution policies produced reserved value double.PositiveInfinity.");
            }
            weightSum += weight;
            weights[i] = weight;
        }
        if (weightSum == 0)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = 1;
            }
            weightSum = weights.Length;
        }
    }
    private void DistributeRemainder<T>(Span<T> targetValues, Span<T> distributedValues, Span<double> weights, int remainder) where T : INumber<T>
    {
        // remainder originates solely from truncation, so each index should only receive at most one additional incrementation

        Span<double> demands = weights.Length <= 256 ? stackalloc double[weights.Length] : new double[weights.Length];
        for (int i = 0; i < demands.Length; i++) demands[i] = weights[i] / (double.CreateSaturating(distributedValues[i]) + 1);
        PriorityQueue<int, double> highestDemandIndices = new PriorityQueue<int, double>();
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
}

using System.Numerics;

namespace Randomness.Distributing;

// always iterates over weights once to validate weights, so summing weights even if the user precomputes is very minimal overhead and fine

/// <summary>
/// An interfacable rendition of the value distribution backbone for all weight-generating distributors. Can be used to pass one's own weights through the distribution algorithms.
/// </summary>
/// <remarks>
/// None of this struct's public methods are thread-safe.
/// </remarks>
public readonly struct Distributor
{
    public void Distribute(int value, Span<double> weights, Span<int> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public int[] Distribute(int value, Span<double> weights)
    {
        int[] target = new int[weights.Length];
        Distribute(value, weights, target);
        return target;
    }
    public void DistributeApproximate(int value, Span<double> weights, Span<int> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public int[] DistributeApproximate(int value, Span<double> weights)
    {
        int[] target = new int[weights.Length];
        DistributeApproximate(value, weights, target);
        return target;
    }
    public void Distribute(long value, Span<double> weights, Span<long> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public long[] Distribute(long value, Span<double> weights)
    {
        long[] target = new long[weights.Length];
        Distribute(value, weights, target);
        return target;
    }
    public void DistributeApproximate(long value, Span<double> weights, Span<long> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public long[] DistributeApproximate(long value, Span<double> weights)
    {
        long[] target = new long[weights.Length];
        DistributeApproximate(value, weights, target);
        return target;
    }
    public void Distribute(nint value, Span<double> weights, Span<nint> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public nint[] Distribute(nint value, Span<double> weights)
    {
        nint[] target = new nint[weights.Length];
        Distribute(value, weights, target);
        return target;
    }
    public void DistributeApproximate(nint value, Span<double> weights, Span<nint> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public nint[] DistributeApproximate(nint value, Span<double> weights)
    {
        nint[] target = new nint[weights.Length];
        DistributeApproximate(value, weights, target);
        return target;
    }
    public void Distribute(double value, Span<double> weights, Span<double> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public double[] Distribute(double value, Span<double> weights)
    {
        double[] target = new double[weights.Length];
        Distribute(value, weights, target);
        return target;
    }
    public void Distribute(decimal value, Span<double> weights, Span<decimal> target)
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public decimal[] Distribute(decimal value, Span<double> weights)
    {
        decimal[] target = new decimal[weights.Length];
        Distribute(value, weights, target);
        return target;
    }
    private void ValidateInputs<T>(Span<double> weights, Span<T> target) where T : INumber<T>
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");
        if (weights.Length < 1) throw new ArgumentException("Weight span must have a length of at least one.");
        if (weights.Length != target.Length) throw new ArgumentException("Weight span and target span must have the same length.");
    }
    private double ValidateAndSumWeights(Span<double> weights)
    {
        double weightSum = 0;
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
                throw new DivideByZeroException("Weight sum cannot be zero when weights can have both signs.");
            }
        }
        return weightSum;
    }
}

using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

// maximum optimizability option, iterate through weights to sum them up only if necessary

/// <summary>
/// An unchecked rendition of the value distribution backbone for all weight-generating distributors. Use it only if you know what you're doing. <para/>
/// Will exhibit rather uninformative exception throwing and erroring behaviors and try to use NaN and infinity in distributing if you let it. Do not.
/// </summary>
/// <remarks>
/// This distributor will still check for the case weightSum == 0, and try to resolve distribution. <para/>
/// Weightless distribution is used when weights don't come in both positive and negative, otherwise an epsilon-based resolution is made. <para/>
/// None of this struct's public methods are thread-safe. <para/>
/// Don't get caught using this struct in production code. ;v
/// </remarks>
public readonly struct UncheckedValueDistributor : IValueDistributor
{
    public void Distribute(int value, Span<double> weights, double weightSum, Span<int> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(int value, Span<double> weights, Span<int> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void DistributeApproximate(int value, Span<double> weights, double weightSum, Span<int> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public void DistributeApproximate(int value, Span<double> weights, Span<int> target)
    {
        double weightSum = SumWeights(weights);
        DistributeApproximate(value, weights, weightSum, target);
    }
    public void Distribute(long value, Span<double> weights, double weightSum, Span<long> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(long value, Span<double> weights, Span<long> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void DistributeApproximate(long value, Span<double> weights, double weightSum, Span<long> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public void DistributeApproximate(long value, Span<double> weights, Span<long> target)
    {
        double weightSum = SumWeights(weights);
        DistributeApproximate(value, weights, weightSum, target);
    }
    public void Distribute(nint value, Span<double> weights, double weightSum, Span<nint> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(nint value, Span<double> weights, Span<nint> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void DistributeApproximate(nint value, Span<double> weights, double weightSum, Span<nint> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEndApprox(value, weights, weightSum, target);
    }
    public void DistributeApproximate(nint value, Span<double> weights, Span<nint> target)
    {
        double weightSum = SumWeights(weights);
        DistributeApproximate(value, weights, weightSum, target);
    }
    public void Distribute(float value, Span<double> weights, double weightSum, Span<float> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(float value, Span<double> weights, Span<float> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void Distribute(double value, Span<double> weights, double weightSum, Span<double> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(double value, Span<double> weights, Span<double> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void Distribute(decimal value, Span<double> weights, double weightSum, Span<decimal> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(decimal value, Span<double> weights, Span<decimal> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void Distribute(NFloat value, Span<double> weights, double weightSum, Span<NFloat> target)
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        DistributorCore.DistrEnd(value, weights, weightSum, target);
    }
    public void Distribute(NFloat value, Span<double> weights, Span<NFloat> target)
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    private static double SumWeights(Span<double> weights)
    {
        double weightSum = 0;
        foreach (double weight in weights) weightSum += weight;
        return weightSum;
    }
}

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
    public void Distribute<T>(T value, Span<double> weights, double weightSum, Span<T> target) where T : unmanaged, INumber<T>
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32:
                DistributorCore.DistrEnd(int.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, int>(target));
                break;
            case TypeCode.Int64:
                DistributorCore.DistrEnd(long.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, long>(target));
                break;
            case TypeCode.Single:
                DistributorCore.DistrEnd(float.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, float>(target));
                break;
            case TypeCode.Double:
                DistributorCore.DistrEnd(double.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, double>(target));
                break;
            case TypeCode.Decimal:
                DistributorCore.DistrEnd(decimal.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, decimal>(target));
                break;
            default:
                if (typeof(T) == typeof(nint))
                {
                    DistributorCore.DistrEnd(nint.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, nint>(target));
                }
                else if (typeof(T) == typeof(NFloat))
                {
                    DistributorCore.DistrEnd(NFloat.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, NFloat>(target));
                }
                else
                {
                    throw new NotSupportedException("Unsupported numeric type.");
                }
                break;
        }
    }
    public void Distribute<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>
    {
        double weightSum = SumWeights(weights);
        Distribute(value, weights, weightSum, target);
    }
    public void DistributeApproximate<T>(T value, Span<double> weights, double weightSum, Span<T> target) where T : unmanaged, INumber<T>
    {
        if (weightSum == 0) DistributorCore.ZeroWeightSum(weights, out weightSum);
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32:
                DistributorCore.DistrEndApprox(int.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, int>(target));
                break;
            case TypeCode.Int64:
                DistributorCore.DistrEndApprox(long.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, long>(target));
                break;
            case TypeCode.Single:
                DistributorCore.DistrEnd(float.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, float>(target));
                break;
            case TypeCode.Double:
                DistributorCore.DistrEnd(double.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, double>(target));
                break;
            case TypeCode.Decimal:
                DistributorCore.DistrEnd(decimal.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, decimal>(target));
                break;
            default:
                if (typeof(T) == typeof(nint))
                {
                    DistributorCore.DistrEndApprox(nint.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, nint>(target));
                }
                else if (typeof(T) == typeof(NFloat))
                {
                    DistributorCore.DistrEnd(NFloat.CreateTruncating(value), weights, weightSum, MemoryMarshal.Cast<T, NFloat>(target));
                }
                else
                {
                    throw new NotSupportedException("Unsupported numeric type.");
                }
                break;
        }
    }
    public void DistributeApproximate<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>
    {
        double weightSum = SumWeights(weights);
        DistributeApproximate(value, weights, weightSum, target);
    }
    private static double SumWeights(Span<double> weights)
    {
        double weightSum = 0;
        foreach (double weight in weights) weightSum += weight;
        return weightSum;
    }
}

using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

// always iterates over weights once to validate weights, so summing weights even if the user precomputes is very minimal overhead and fine

/// <summary>
/// The value distribution backbone for all weight-generating distributors. Can be used to pass one's own weights through the distribution algorithms. <para/>
/// Throws useful <see cref="ArgumentException"/>s and <see cref="ArgumentOutOfRangeException"/>s when used poorly.
/// </summary>
/// <remarks>
/// None of this struct's public methods are thread-safe.
/// </remarks>
public readonly struct ValueDistributor : IValueDistributor
{
    public void Distribute<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
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
    public void DistributeApproximate<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>
    {
        ValidateInputs(weights, target);
        double weightSum = ValidateAndSumWeights(weights);
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
    private void ValidateInputs<T>(Span<double> weights, Span<T> target) where T : INumber<T>
    {
        if (target.Length < 1) throw new ArgumentException("Target span must have a length of at least one.");
        if (weights.Length < 1) throw new ArgumentException("Weight span must have a length of at least one.");
        if (weights.Length != target.Length) throw new ArgumentException("Weight span and target span must have the same length.");
    }
    private double ValidateAndSumWeights(Span<double> weights)
    {
        double weightSum = 0;
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
        return weightSum;
    }
}

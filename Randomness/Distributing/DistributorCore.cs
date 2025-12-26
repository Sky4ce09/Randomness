using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

internal static class DistributorCore
{

    internal static void DistributeRemainder<T>(Span<T> targetValues, Span<T> distributedValues, Span<double> weights, int remainder) where T : INumber<T>
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
                    lowestDemandIndices.TryPeek(out _, out double highestQueuedDemand);
                    if (demand >= highestQueuedDemand) continue;
                    lowestDemandIndices.Dequeue();
                    lowestDemandIndices.Enqueue(i, demand);
                }
            }
            while (lowestDemandIndices.Count > 0) targetValues[lowestDemandIndices.Dequeue()]--;
        }
    }
    internal static void DistrEnd(int value, Span<double> weights, double weightSum, Span<int> target)
    {
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
    internal static void DistrEndApprox(int value, Span<double> weights, double weightSum, Span<int> target)
    {
        double doubleValue = Convert.ToDouble(value);
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += (int)Math.Round(doubleValue * weights[i] / weightSum);
        }
    }
    internal static void DistrEnd(long value, Span<double> weights, double weightSum, Span<long> target)
    {
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
    internal static void DistrEndApprox(long value, Span<double> weights, double weightSum, Span<long> target)
    {
        double doubleValue = Convert.ToDouble(value);
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += (long)Math.Round(doubleValue * weights[i] / weightSum);
        }
    }
    internal static void DistrEnd(nint value, Span<double> weights, double weightSum, Span<nint> target)
    {
        // distribute value
        Span<nint> distributed = target.Length <= 256 ? stackalloc nint[target.Length] : new nint[target.Length];
        double doubleValue = Convert.ToDouble(value);
        nint totalDistributedValue = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            nint distribution = (nint)(doubleValue * weights[i] / weightSum);
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
    internal static void DistrEndApprox(nint value, Span<double> weights, double weightSum, Span<nint> target)
    {
        double doubleValue = Convert.ToDouble(value);
        for (int i = 0; i < weights.Length; i++)
        {
            target[i] += (nint)Math.Round(doubleValue * weights[i] / weightSum);
        }
    }
    internal static void DistrEnd(float value, Span<double> weights, double weightSum, Span<float> target)
    {
        float distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            float share = value * (float)weights[i] / (float)weightSum;
            target[i] += share;
            distributed += share;
        }
    }
    internal static void DistrEnd(double value, Span<double> weights, double weightSum, Span<double> target)
    {
        double distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            double share = value * weights[i] / weightSum;
            target[i] += share;
            distributed += share;
        }
    }
    internal static void DistrEnd(decimal value, Span<double> weights, double weightSum, Span<decimal> target)
    {
        decimal distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            decimal share = value * (decimal)weights[i] / (decimal)weightSum;
            target[i] += share;
            distributed += share;
        }
    }
    internal static void DistrEnd(NFloat value, Span<double> weights, double weightSum, Span<NFloat> target)
    {
        NFloat distributed = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            NFloat share = value * (NFloat)weights[i] / (NFloat)weightSum;
            target[i] += share;
            distributed += share;
        }
    }
    internal static void SpreadEven(Span<double> weights, ref double weightSum)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = 1;
        }
        weightSum = weights.Length;
    }
    internal static void ZeroWeightSum(Span<double> weights, out double weightSum)
    {
        double largestAbsWeight = double.NegativeInfinity;
        Span<bool> signsEncountered = stackalloc bool[] { false, false }; // positive, negative
        for (int i = 0; i < weights.Length; i++)
        {
            double weight = weights[i];
            double absWeight = Math.Abs(weight);
            if (absWeight > largestAbsWeight) largestAbsWeight = absWeight;
            signsEncountered[weight < 0 ? 1 : 0] = true;
        }
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
}

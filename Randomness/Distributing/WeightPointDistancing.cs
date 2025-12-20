using System.Numerics;

namespace Randomness.Distributing;

/// <summary>
/// An exemplary <see cref="IWeightDistributionPolicy"/> implementer that linearly maps weights from one area around a point to another in 1D space. <para/>
/// Minimum distances from points can be specified. Works with negative weights.
/// </summary>
public struct WeightPointDistancing : IWeightDistributionPolicy
{
    public double SourcePoint { get; set; }
    public double SourceMinDistance { get; set; }
    public double SourceMaxDistance { get; set; }
    public double TargetPoint { get; set; }
    public double TargetMinDistance { get; set; }
    public double TargetMaxDistance { get; set; }

    /// <summary>
    /// Initializes a new <see cref="WeightPointDistancing"/> with points set to 0 and distance ranges set to [0, 1). <para/>
    /// This policy will have no effect on weights that doesn't stem from rounding errors unless reconfigured.
    /// </summary>
    public WeightPointDistancing()
    {
        SourceMinDistance = 0;
        SourceMaxDistance = 1;
        SourcePoint = 0;
        TargetMinDistance = 0;
        TargetMaxDistance = 1;
        TargetPoint = 0;
    }
    /// <summary>
    /// Initializes a new <see cref="WeightPointDistancing"/> with the source point set to 0 and the source distance range set to [0, 1). <para/>
    /// The target point and distance range are set by the provided parameters <paramref name="point"/>, <paramref name="minDistance"/>, <paramref name="maxDistance"/>. <para/>
    /// </summary>
    /// <param name="point">The point around which weights will be mapped.</param>
    /// <param name="minDistance">The minimum distance weights will have from the specified point after mapping.</param>
    /// <param name="maxDistance">The maximum distance weights will have from the specified point after mapping.</param>
    public WeightPointDistancing(double point, double minDistance, double maxDistance)
    {
        SourcePoint = 0;
        SourceMinDistance = 0;
        SourceMaxDistance = 1;
        TargetPoint = point;
        TargetMinDistance = minDistance;
        TargetMaxDistance = maxDistance;
    }
    /// <summary>
    /// Initializes a new <see cref="WeightPointDistancing"/> with <paramref name="sourcePoint"/>, <paramref name="sourceMinDistance"/>, <paramref name="sourceMaxDistance"/>,
    /// <paramref name="targetPoint"/>, <paramref name="targetMinDistance"/>, <paramref name="targetMaxDistance"/> specified.
    /// </summary>
    /// <param name="sourcePoint">The point around which weights are assumed to be in.</param>
    /// <param name="sourceMinDistance">The minimum distance the weights are assumed to have from the source point.</param>
    /// <param name="sourceMaxDistance">The maximum distance the weights are assumed to have from the source point.</param>
    /// <param name="targetPoint">The point around which weights will be mapped.</param>
    /// <param name="targetMinDistance">The minimum distance weights will have from the specified target point after mapping.</param>
    /// <param name="targetMaxDistance">The maximum distance weights will have from the specified target point after mapping.</param>
    public WeightPointDistancing(double sourcePoint, double sourceMinDistance, double sourceMaxDistance, double targetPoint, double targetMinDistance, double targetMaxDistance)
    {
        SourcePoint = sourcePoint;
        SourceMinDistance = sourceMinDistance;
        SourceMaxDistance = sourceMaxDistance;
        TargetPoint = targetPoint;
        TargetMinDistance = targetMinDistance;
        TargetMaxDistance = targetMaxDistance;
    }
    public readonly void ApplyTo(Span<double> weights)
    {
        double fromPoint = SourcePoint;
        double fromMinDist = SourceMinDistance;
        double fromMaxDist = SourceMaxDistance;
        double toPoint = TargetPoint;
        double toMinDist = TargetMinDistance;
        double toMaxDist = TargetMaxDistance;

        double fromSpan = fromMaxDist - fromMinDist;
        double toSpan = toMaxDist - toMinDist;
        double toOffset = toPoint + toMinDist;

        if (fromSpan != 0)
        {
            double fromOffset = fromPoint + fromMinDist;
            for (int i = 0; i < weights.Length; i++)
            {
                double weight = weights[i];
                weight -= fromOffset;
                weight /= fromSpan;
                weight *= toSpan;
                weight += toOffset * (((BitConverter.DoubleToInt64Bits(weight) >> 63) << 1) + 1);
                weights[i] = weight;
            }
        }
        else
        {
            double midpoint = toSpan * 0.5 + toOffset;
            weights.Fill(midpoint);
        }
    }
}
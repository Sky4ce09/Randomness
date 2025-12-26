using Randomness.Sampling;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

public class ConfigurableDistributor
{
    public ISampler Sampler { get; set; }
    public IValueDistributor Distributor { get; set; }

    public ConfigurableDistributor(ISampler? sampler = null, IValueDistributor? distributor = null)
    {
        Sampler = sampler ?? new WhiteNoise();
        Distributor = distributor ?? new ValueDistributor();
    }

    /// <summary>
    /// Distributes a number <paramref name="value"/> across <paramref name="target"/>, leaving no remainder. <para/>
    /// Throws a <see cref="NotSupportedException"/> for unsigned types and bytes/shorts. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty.
    /// </summary>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied before value distribution.</param>
    /// <exception cref="NotSupportedException">Thrown when any unsigned type or byte/short is specified as the type argument.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="target"/> is empty.</exception>
    public void Distribute<T>(T value, Span<T> target, params IWeightDistributionPolicy[] weightModifiers) where T : unmanaged, INumber<T>
    {
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        Sampler.AddSample(weights);
        foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);
        Distributor.Distribute(value, weights, target);
    }

    /// <summary>
    /// Distributes an integral number <paramref name="value"/> across <paramref name="target"/> using weighted proportions and per-element rounding. <para/>
    /// Throws a <see cref="NotSupportedException"/> for unsigned types and bytes/shorts. <para/>
    /// Throws an <see cref="ArgumentException"/> if the target span is empty.
    /// </summary>
    /// <remarks>
    /// This method doesn't guarantee that the sum of distributed values equals <paramref name="value"/>. Each element is rounded independently, which may introduce a cumulative rounding error. <para/>
    /// Use <see cref="Distribute"/> when exact conservation of the input value is required.
    /// </remarks>
    /// <param name="weightModifiers">The <see cref="IWeightDistributionPolicy"/> implementers to be applied before value distribution.</param>
    /// <exception cref="NotSupportedException">Thrown when any unsigned type or byte/short is specified as the type argument.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="target"/> is empty.</exception>
    public void DistributeApproximate<T>(T value, Span<T> target, params IWeightDistributionPolicy[] weightModifiers) where T : unmanaged, INumber<T>
    {
        Span<double> weights = target.Length <= 256 ? stackalloc double[target.Length] : new double[target.Length];
        Sampler.AddSample(weights);
        foreach (IWeightDistributionPolicy weightModifier in weightModifiers) weightModifier.ApplyTo(weights);
        Distributor.DistributeApproximate(value, weights, target);
    }
}

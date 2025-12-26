using Randomness.Sampling;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Randomness.Distributing;

/// <summary>
/// A noise sampler / distributor that distributes a number value across multiple elements in a span using weighted distribution. <para/>
/// Sources weights from a noise map using FastNoiseLite 1.1.1 (configurable through the Sampler property).
/// </summary>
public sealed class NoiseDistributor : ConfigurableDistributor
{
    public NoiseDistributor() : base(new CoherentNoise(), new ValueDistributor()) { }
    public NoiseDistributor(CoherentNoise? sampler = null, IValueDistributor? distributor = null) : base(sampler ?? new CoherentNoise(), distributor ?? new ValueDistributor()) { }

    /// <summary>
    /// Sets the size and dimensions of the sampled space.
    /// </summary>
    /// <param name="x">The width argument.</param>
    /// <param name="y">The height argument. Leave unspecified or set to 0 for 1D sampling.</param>
    /// <param name="z">The depth argument. Leave unspecified or set to 0 for 2D sampling.</param>
    public NoiseDistributor SetDimensions(float x, float y = 0.0f, float z = 0.0f)
    {
        if (Sampler is CoherentNoise s) s.SetDimensions(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets the seed of the noise sampler.
    /// </summary>
    /// <param name="seed">The seed to use for noise sampling.</param>
    public NoiseDistributor SetSeed(int seed)
    {
        if (Sampler is CoherentNoise s) s.Seed = seed;
        return this;
    }

    /// <summary>
    /// Sets the size and dimensions of the sampled space.
    /// </summary>
    public NoiseDistributor SetDimensions(Vector3 dimensions) { return SetDimensions(dimensions.X, dimensions.Y, dimensions.Z); }

    /// <summary>
    /// Sets the spatial distances between individual sampling operations. Low values "zoom in", high values "zoom out" of the noise map.
    /// </summary>
    /// <param name="x">The x-coordinate scale.</param>
    /// <param name="y">The y-coordinate scale.</param>
    /// <param name="z">The z-coordinate scale.</param>
    public NoiseDistributor SetScale(float x, float y = 1.0f, float z = 1.0f)
    {
        if (Sampler is CoherentNoise s) s.SetScale(x, y, z);
        return this;
    }
    public NoiseDistributor SetScale(Vector3 scale) { return SetScale(scale.X, scale.Y, scale.Z); }

    /// <summary>
    /// Sets the position within the noise map where sampling is started.
    /// </summary>
    /// <param name="x">The x-coordinate for the weight generation algorithm to start sampling from. (1D, 2D, 3D)</param>
    /// <param name="y">The y-coordinate for the weight generation algorithm to start sampling from. (2D, 3D)</param>
    /// <param name="z">The z-coordinate for the weight generation algorithm to start sampling from. (3D)</param>
    public NoiseDistributor SetPosition(float x, float y = 0.0f, float z = 0.0f)
    {
        if (Sampler is CoherentNoise s) s.SetPosition(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets the position within the noise map where sampling is started.
    /// </summary>
    public NoiseDistributor SetPosition(Vector3 position) { return SetPosition(position.X, position.Y, position.Z); }

    /// <summary>
    /// Sets a FastNoiseLite supported noise type.
    /// </summary>
    /// <param name="noiseType">The noise type to sample from.</param>
    public NoiseDistributor SetNoiseType(FastNoiseType noiseType)
    {
        if (Sampler is CoherentNoise s) s.SetNoiseType(noiseType);
        return this;
    }
}

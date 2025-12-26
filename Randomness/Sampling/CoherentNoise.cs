using System.Numerics;

namespace Randomness.Sampling;

/// <summary>
/// A configurable noise sampling layer that interacts with FastNoiseLite to get coherent noise values.
/// </summary>
public class CoherentNoise : ISampler
{
    /// <summary>
    /// The seed used in determining noise generation.
    /// </summary>
    public int Seed { set { NoiseGenerator.SetSeed(value); } }

    /// <summary>
    /// The dimensions which describe the sampled noise boundaries. <para/>
    /// For one-dimensional noise, set Y and Z to 0. <para/>
    /// For two-dimensional noise, set Z to 0. <para/>
    /// For three-dimensional noise, set all as >= 1.
    /// </summary>
    public Vector3 NoiseDimensions { get; set; }

    /// <summary>
    /// Per-axis scaling factors. Use small factors to "zoom in" to the noise map, large factors to "zoom out" of the noise map.
    /// </summary>
    public Vector3 NoiseScalingFactors { get; set; }

    /// <summary>
    /// The position within the noise map where sampling is started.
    /// </summary>
    public Vector3 NoiseSamplingPosition { get; set; }

    private FastNoiseLite NoiseGenerator { get; set; }

    /// <summary>
    /// Initializes a new <see cref="CoherentNoise"/> instance and sets its noise generator's seed.
    /// </summary>
    /// <param name="seed"></param>
    public CoherentNoise(int seed)
    {
        NoiseGenerator = new FastNoiseLite(seed);
        NoiseGenerator.SetFrequency(1.0f);
        NoiseDimensions = new Vector3(1, 1, 0);
        NoiseScalingFactors = new Vector3(1, 1, 1);
        NoiseSamplingPosition = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Initializes a new <see cref="CoherentNoise"/> instance with default values.
    /// </summary>
    public CoherentNoise()
    {
        NoiseGenerator = new FastNoiseLite();
        NoiseGenerator.SetFrequency(1.0f);
        NoiseDimensions = new Vector3(1, 1, 0);
        NoiseScalingFactors = new Vector3(1, 1, 1);
        NoiseSamplingPosition = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Sets the size and dimensions of the sampled space.
    /// </summary>
    /// <param name="x">The width argument.</param>
    /// <param name="y">The height argument. Leave unspecified or set to 0 for 1D sampling.</param>
    /// <param name="z">The depth argument. Leave unspecified or set to 0 for 2D sampling.</param>
    public CoherentNoise SetDimensions(float x, float y = 0.0f, float z = 0.0f)
    {
        NoiseDimensions = new Vector3(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets the spatial distances between individual sampling operations.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public CoherentNoise SetScale(float x, float y = 1.0f, float z = 1.0f)
    {
        NoiseScalingFactors = new Vector3(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets the position within the noise map where sampling is started.
    /// </summary>
    /// <param name="x">The x-coordinate for the weight generation algorithm to start sampling from. (1D, 2D, 3D)</param>
    /// <param name="y">The y-coordinate for the weight generation algorithm to start sampling from. (2D, 3D)</param>
    /// <param name="z">The z-coordinate for the weight generation algorithm to start sampling from. (3D)</param>
    public CoherentNoise SetPosition(float x, float y = 0.0f, float z = 0.0f)
    {
        NoiseSamplingPosition = new Vector3(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets a FastNoiseLite supported noise type.
    /// </summary>
    /// <param name="noiseType">The noise type to sample from.</param>
    public CoherentNoise SetNoiseType(FastNoiseType noiseType)
    {
        NoiseGenerator.SetNoiseType((FastNoiseLite.NoiseType)noiseType);
        return this;
    }

    /// <summary>
    /// Samples noise and adds it to the provided span.
    /// </summary>
    /// <param name="outValues"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public double? AddSample(Span<double> outValues)
    {
        double sum = 0;

        if (outValues.Length < 1) throw new ArgumentException("Weights span must have a length of at least one.");
        if (NoiseDimensions.X < 1) throw new ArgumentException("First noise dimension (X) must be 1 or greater.");

        int totalWeightCount = (int)NoiseDimensions.X * Math.Max(1, (int)NoiseDimensions.Y) * Math.Max(1, (int)NoiseDimensions.Z);
        if (outValues.Length < totalWeightCount) throw new ArgumentException("Not enough space for dimensions to generate. (weights.Length must be >= all dimensions multiplied)");

        int dimensionCount = 1 + (NoiseDimensions.Y >= 1 ? 1 : 0) + (NoiseDimensions.Z >= 1 ? 1 : 0);
        int weightIndex = 0;
        switch (dimensionCount)
        {
            case 1:
                for (float x = NoiseSamplingPosition.X; x < (int)NoiseDimensions.X + NoiseSamplingPosition.X; x++)
                {
                    outValues[weightIndex] += NoiseGenerator.GetNoise(x * NoiseScalingFactors.X, 0);
                    weightIndex++;
                }
                break;
            case 2:
                for (float y = NoiseSamplingPosition.Y; y < (int)NoiseDimensions.Y + NoiseSamplingPosition.Y; y++)
                {
                    for (float x = NoiseSamplingPosition.X; x < (int)NoiseDimensions.X + NoiseSamplingPosition.X; x++)
                    {
                        outValues[weightIndex] += NoiseGenerator.GetNoise(x * NoiseScalingFactors.X, y * NoiseScalingFactors.Y);
                        weightIndex++;
                    }
                }
                break;
            case 3:
                for (float z = NoiseSamplingPosition.Z; z < (int)NoiseDimensions.Z + NoiseSamplingPosition.Z; z++)
                {
                    for (float y = NoiseSamplingPosition.Y; y < (int)NoiseDimensions.Y + NoiseSamplingPosition.Y; y++)
                    {
                        for (float x = NoiseSamplingPosition.X; x < (int)NoiseDimensions.X + NoiseSamplingPosition.X; x++)
                        {
                            outValues[weightIndex] += NoiseGenerator.GetNoise(x * NoiseScalingFactors.X, y * NoiseScalingFactors.Y, z * NoiseScalingFactors.Z);
                            weightIndex++;
                        }
                    }
                }
                break;
        }

        return sum;
    }
}

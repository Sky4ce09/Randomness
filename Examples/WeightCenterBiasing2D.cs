namespace Randomness.Distributing;

public readonly struct WeightCenterBiasing2D : IWeightDistributionPolicy
{
    private readonly int[] _dimensions;
    public WeightCenterBiasing2D(params int[] dimensions)
    {
        _dimensions = dimensions;
    }
    public readonly void ApplyTo(Span<double> weights)
    {
        // this will error with inaccurate dimension configurations so please keep that in mind when ripping from this snippet
        int width = _dimensions[0];
        int height = _dimensions[1];
        double centerX = (width - 1) * 0.5;
        double centerY = (height - 1) * 0.5;

        double radius = Math.Sqrt(centerX * centerX + centerY * centerY);

        int weightIndex = 0;
        for (int y = 0; y < height; y++)
        {
            double deltaY = y - centerY;

            for (int x = 0; x < width; x++)
            {
                double deltaX = x - centerX;
                double dist = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                double linearFalloff = 1.0 - dist / radius;
                double scale = Math.Max(0, linearFalloff * linearFalloff);

                weights[weightIndex] *= scale;
                weightIndex++;
            }
        }
    }
}

using Randomness.Distributing;

namespace Examples.Usage;

internal class DistributingExamples
{
    public static void RunExamples()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("RandomDistributor class\n");
        Console.ForegroundColor = ConsoleColor.White;
        BasicExample();
        IWeightDistributionExample();
        CustomImplementerExample();
        TwoDimExample();
        NegativeWeightsExample();
        PointDistanceExample();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("NoiseDistributor class\n");
        Console.ForegroundColor = ConsoleColor.White;
        TwoDimNoiseExample();
    }
    static void BasicExample()
    {
        Console.WriteLine("1D Array example");

        int trials = 5;
        int distributedValue = 15;
        int[] target = new int[5];

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0);
            new RandomDistributor().Distribute<int>(distributedValue, target);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void IWeightDistributionExample()
    {
        Console.WriteLine("IWeightDistributionPolicy example, mapping weights to a range");

        int trials = 5;
        double distributedValue = 15.0D;
        double[] target = new double[5];
        double toLowerBound = 0.3;
        double toUpperBound = 0.7;
        IWeightDistributionPolicy policy = new WeightRangeMapping(fromLowerBound: 0, fromUpperBound: 1, toLowerBound, toUpperBound);

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(0, 1, {0}, {1})", toLowerBound, toUpperBound));

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0.0D);
            new RandomDistributor().Distribute<double>(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void CustomImplementerExample()
    {
        Console.WriteLine("Custom IWeightDistributionPolicy implementer example, clamping weights to a range");

        int trials = 5;
        double distributedValue = 15.0D;
        double[] target = new double[5];
        double lowerBound = 0.4;
        double upperBound = 0.6;
        IWeightDistributionPolicy policy = new WeightClamping(lowerBound, upperBound);

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightClamping({0}, {1})", lowerBound, upperBound));

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0.0D);
            new RandomDistributor().Distribute<double>(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void TwoDimExample()
    {
        Console.WriteLine("\"2D Array\" example with center biasing");

        int trials = 2;
        int distributedValue = 30;
        int width = 7;
        int height = 7;
        double toLowerBound = 0.6;
        double toUpperBound = 1;
        int[] target = new int[width * height];
        IWeightDistributionPolicy[] policies = [
            new WeightRangeMapping(fromLowerBound: 0, fromUpperBound: 1, toLowerBound, toUpperBound),
            new WeightCenterBiasing2D(width, height)
            ];

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(0, 1, {0}, {1})", toLowerBound, toUpperBound));
        Console.WriteLine("WeightCenterBiasing2D");

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Console.WriteLine("Trial " + (trial + 1));
            Array.Fill(target, 0);
            new RandomDistributor().Distribute<int>(distributedValue, target, policies);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(target[y * width + x] + ", ");
                }
                Console.WriteLine();
            }
        }
        End();
    }
    static void NegativeWeightsExample()
    {
        Console.WriteLine("Negative weights example");

        int trials = 5;
        int distributedValue = 15;
        double toLowerBound = -0.5;
        double toUpperBound = 1;
        int[] target = new int[5];
        IWeightDistributionPolicy policy = new WeightRangeMapping(fromLowerBound: 0, fromUpperBound: 1, toLowerBound, toUpperBound);

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(0, 1, {0}, {1})", toLowerBound, toUpperBound));

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0);
            new RandomDistributor().DistributeApproximate<int>(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void PointDistanceExample()
    {
        Console.WriteLine("Sign-compatible point distance example");

        int trials = 5;
        int distributedValue = 15;
        double toLowerBound = -1;
        double toUpperBound = 1;
        double targetMinDistance = 0.5;
        double targetMaxDistance = 1;
        int[] target = new int[5];
        IWeightDistributionPolicy[] policies = [
            new WeightRangeMapping(fromLowerBound: 0, fromUpperBound: 1, toLowerBound, toUpperBound),
            new WeightPointDistancing(sourcePoint: 0, sourceMinDistance: 0, sourceMaxDistance: 1, targetPoint: 0, targetMinDistance, targetMaxDistance)
            ];

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(0, 1, {0}, {1})", toLowerBound, toUpperBound));
        Console.WriteLine(string.Format("WeightPointDistancing(0, 0, 1, 0, {0}, {1})", targetMinDistance, targetMaxDistance));

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0);
            new RandomDistributor().DistributeApproximate<int>(distributedValue, target, policies);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void TwoDimNoiseExample()
    {
        Console.WriteLine("\"2D Array\" example using FastNoiseLite with weights sampled from 2D simplex noise");
        Console.WriteLine("(other noise types are also supported)");
        Console.WriteLine("I am not sure why you would use noise generated weights for your distribution, but it's an option now");

        int width = 12;
        int height = 12;
        double toLowerBound = 0;
        double toUpperBound = 1;
        IWeightDistributionPolicy policy = new WeightRangeMapping(fromLowerBound: -1, fromUpperBound: 1, toLowerBound, toUpperBound);

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(-1, 1, {0}, {1})", toLowerBound, toUpperBound));

        int distributedValue = 400;
        int[] target = new int[width * height];

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        new NoiseDistributor().SetDimensions(width, height).SetScale(0.05f, 0.05f, 0.05f).Distribute(distributedValue, target, policy);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Console.Write(target[y * width + x] + ", ");
            }
            Console.WriteLine();
        }
        End();
    }
    private static void End() { Console.WriteLine("\n-----------\n"); }
}

using Randomness.Distributing;

namespace Examples.Usage;

internal class DistributingExamples
{
    static RandomDistributor distributor;
    public static void RunExamples()
    {
        distributor = new RandomDistributor();

        BasicExample();
        IWeightDistributionExample();
        CustomImplementerExample();
        TwoDimTensorExample();
        NegativeWeightsExample();
    }
    static void BasicExample()
    {
        Console.WriteLine("1D Array example with integers");

        int trials = 5;
        int distributedValue = 15;
        int[] target = new int[5];

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0);
            distributor.DistributeApproximate(distributedValue, target);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void IWeightDistributionExample()
    {
        Console.WriteLine("IWeightDistributionPolicy example with doubles, mapping weights to a range");

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
            distributor.Distribute(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void CustomImplementerExample()
    {
        Console.WriteLine("Custom IWeightDistributionPolicy implementer example with doubles, clamping weights to a range");

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
            distributor.Distribute(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void TwoDimTensorExample()
    {
        Console.WriteLine("\"2D Array\" example with integer and center biasing");

        int trials = 2;
        int distributedValue = 20;
        int width = 7;
        int height = 7;
        double toLowerBound = 0.6;
        double toUpperBound = 1;
        int[] target = new int[width * height];
        IWeightDistributionPolicy[] policies = [new WeightRangeMapping(fromLowerBound: 0, fromUpperBound: 1, toLowerBound, toUpperBound), new WeightCenterBiasing2D(width, height)];

        Console.WriteLine("\nApplied IWeightDistributionPolicy implementers:");
        Console.WriteLine(string.Format("WeightRangeMapping(0, 1, {0}, {1})", toLowerBound, toUpperBound));
        Console.WriteLine("WeightCenterBiasing2D");

        Console.WriteLine(string.Format("\nValue: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Console.WriteLine("Trial " + (trial + 1));
            Array.Fill(target, 0);
            distributor.DistributeApproximate(distributedValue, target, policies);
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
        Console.WriteLine("Negative weights example with integers");

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
            distributor.DistributeApproximate(distributedValue, target, policy);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    private static void End() { Console.WriteLine("\n-----------\n"); }
}

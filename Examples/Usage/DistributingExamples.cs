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
    }
    static void BasicExample()
    {
        Console.WriteLine("1D Array example with integers\n");

        int trials = 5;
        int distributedValue = 15;
        int[] target = new int[5];

        Console.WriteLine(string.Format("Value: {0} | Spread: {1}", distributedValue, target.Length));

        for (int trial = 0; trial < trials; trial++)
        {
            Array.Fill(target, 0);
            distributor.Distribute(distributedValue, target);
            Console.WriteLine(string.Format("Trial {0}: {1}", trial + 1, string.Join(", ", target)));
        }
        End();
    }
    static void IWeightDistributionExample()
    {
        Console.WriteLine("IWeightDistributionPolicy example with doubles, mapping weights to a range\n");

        int trials = 5;
        double distributedValue = 15.0D;
        double[] target = new double[5];
        double lowerBound = 0.3;
        double upperBound = 0.7;
        IWeightDistributionPolicy policy = new WeightRangeMapping(lowerBound, upperBound);

        Console.WriteLine(string.Format(
            "Value: {0} | Spread: {1} | Lower weight bound: {2} | Upper weight bound: {3}",
            distributedValue, target.Length, lowerBound, upperBound
            ));

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
        Console.WriteLine("Custom IWeightDistributionPolicy implementer example with doubles, clamping weights to a range\n");

        int trials = 5;
        double distributedValue = 15.0D;
        double[] target = new double[5];
        double lowerBound = 0.4;
        double upperBound = 0.6;
        IWeightDistributionPolicy policy = new WeightClamping(lowerBound, upperBound);

        Console.WriteLine(string.Format(
            "Value: {0} | Spread: {1} | Lower weight bound: {2} | Upper weight bound: {3}",
            distributedValue, target.Length, lowerBound, upperBound
            ));

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
        Console.WriteLine("\"2D Array\" example with integer\nand a tensor access scaffold over a 1D Array of weights in a custom IWeightDistributionPolicy implementer");
        Console.WriteLine("biasing weights towards the center of the array\n");

        int trials = 2;
        int distributedValue = 20;
        int width = 7;
        int height = 7;
        double lowerBound = 0.6;
        double upperBound = 1;
        int[] target = new int[width * height];
        IWeightDistributionPolicy[] policies = [new WeightRangeMapping(lowerBound, upperBound), new WeightCenterBiasing2D(width, height)];

        Console.WriteLine(string.Format(
            "Value: {0} | Spread: {1} | Pre-bias Lower weight bound: {2} | Pre-bias Upper weight bound: {3}",
            distributedValue, target.Length, lowerBound, upperBound
            ));

        for (int trial = 0; trial < trials; trial++)
        {
            Console.WriteLine("Trial " + (trial + 1));
            Array.Fill(target, 0);
            distributor.Distribute(distributedValue, target, policies);
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
    private static void End() { Console.WriteLine("\n-----------\n"); }
}

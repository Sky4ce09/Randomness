using Randomness.Distributing;
using Randomness.Sampling;

namespace Examples.Usage
{
    internal class SamplingExamples
    {
        public static void RunExamples()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CoherentNoise class\n");
            Console.ForegroundColor = ConsoleColor.White;
            SimplexExample();
            SmoothSimplexExample();
            PerlinExample();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WhiteNoise class\n");
            Console.ForegroundColor = ConsoleColor.White;
            WhiteNoiseExample();
        }
        internal static void SimplexExample()
        {
            Console.WriteLine("Simplex Noise Example");

            int width = 15;
            int height = 15;
            double[] noiseValues = new double[width * height];

            new CoherentNoise().SetDimensions(width, height).SetScale(0.05f, 0.05f, 0.05f).AddSample(noiseValues);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write((noiseValues[y * width + x] > 0 ? "+" : "") + Math.Round(noiseValues[y * width + x], 2) + ", ");
                }
                Console.WriteLine();
            }
            End();
        }
        internal static void SmoothSimplexExample()
        {
            Console.WriteLine("Smooth Simplex Noise Example");

            int width = 15;
            int height = 15;
            double[] noiseValues = new double[width * height];

            // compared to previous example: set the noise type to OpenSimplex2S
            new CoherentNoise().SetNoiseType(FastNoiseType.OpenSimplex2S).SetDimensions(width, height).SetScale(0.05f, 0.05f, 0.05f).AddSample(noiseValues);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write((noiseValues[y * width + x] > 0 ? "+" : "") + Math.Round(noiseValues[y * width + x], 2) + ", ");
                }
                Console.WriteLine();
            }
            End();
        }
        internal static void PerlinExample()
        {
            Console.WriteLine("Perlin Noise Example");

            int width = 15;
            int height = 15;
            double[] noiseValues = new double[width * height];

            // compared to previous example: set the noise type to Perlin
            new CoherentNoise().SetNoiseType(FastNoiseType.Perlin).SetDimensions(width, height).SetScale(0.05f, 0.05f, 0.05f).AddSample(noiseValues);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write((noiseValues[y * width + x] >= 0 ? "+" : "") + Math.Round(noiseValues[y * width + x], 2) + ", ");
                }
                Console.WriteLine();
            }
            End();
        }
        internal static void WhiteNoiseExample()
        {
            Console.WriteLine("White Noise Example");

            int width = 15;
            int height = 15;
            double[] noiseValues = new double[width * height];

            new WhiteNoise().AddSample(noiseValues);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(Math.Round(noiseValues[y * width + x], 2) + ", ");
                }
                Console.WriteLine();
            }
            End();
        }
        private static void End() { Console.WriteLine("\n-----------\n"); }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Examples.Usage;

internal class Program
{
    static void Main(string[] args)
    {
        RunDistributingExamples();
    }
    static void RunDistributingExamples()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Examples: Randomness.Distributing\n");
        Console.ForegroundColor = ConsoleColor.White;
        DistributingExamples.RunExamples();
    }
}
# Randomness
![NuGet](https://img.shields.io/nuget/v/Randomness)

WIP library containing randomization algorithms

## Targets .NET 8

### Installation

```bash
dotnet add package Randomness
```

### Includes solutions for...

* Gathering large samples of coherent noise (using FastNoiseLite) or white (statistically random) noise
* Weighted distribution of number values across multiple other numbers using said noise
* More to be added!

### Usage Examples

#### [Randomness.Distributing](https://github.com/Sky4ce09/Randomness/blob/master/Examples/Usage/DistributingExamples.cs)

```cs
int value = 100;
int[] target = new int[20];

var distributor = new RandomDistributor();
distributor.Distribute(value, target);

// the sum of all numbers in the target array is now 100
```

#### [Randomness.Sampling](https://github.com/Sky4ce09/Randomness/blob/master/Examples/Usage/SamplingExamples.cs)

```cs
int width = 20;
int height = 20;
double[] noiseValues = new double[width * height];
CoherentNoise sampler = new CoherentNoise()
    .SetNoiseType(FastNoiseType.OpenSimplex2S)
    .SetDimensions(width, height) // two-dimensional noise (20x20)
    .SetScale(0.05f, 0.05f); // one scale for every coordinate
sampler.AddSample(noiseValues); // fills noiseValues
// individual values range from -1 to 1

// interop with Randomness.Distributing:
int value = 2000;
var distributor = new ValueDistributor();
int[] distributedValues = new int[width * height];
distributor.Distribute(value, noiseValues, distributedValues);
```

### Authors

Sky4ce09

### Links

[GitHub Wiki](https://github.com/Sky4ce09/Randomness/wiki)
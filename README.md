# Randomness
![NuGet](https://img.shields.io/nuget/v/Randomness)

WIP library containing randomization algorithms

## Targets .NET 8

### Installation

```bash
dotnet add package Randomness
```

### Includes solutions for...

* Gathering large samples of coherent noise (using FastNoiseLite) or white noise
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

#### [Randomness.Sampling](https://github.com/Sky4ce09/Randomness/blob/master/Examples/Usage/DistributingExamples.cs)

```cs
int value = 100;
int[] target = new int[20];

var distributor = new RandomDistributor();
distributor.Distribute(value, target);

// the sum of all numbers in the target array is now 100
```

### Authors

Sky4ce09

### Links

[GitHub Wiki](https://github.com/Sky4ce09/Randomness/wiki)
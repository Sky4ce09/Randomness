# Randomness
![NuGet](https://img.shields.io/nuget/v/Randomness)

WIP NuGet-Package containing randomization algorithms for .NET

## Targets .NET 8

### Installation

```bash
dotnet add package Randomness
```

### Includes solutions for...

* Weighted distribution of number values across multiple other numbers
* More to be added!

### Usage Examples

#### Randomness.Distributing

```cs
int value = 100;
int[] target = new int[20];

var distributor = new ValueDistributor();
distributor.Distribute(value, target);

// want to map Random.NextDouble() weights to a different range?
double minWeight = double.E;
double maxWeight = double.Pi;
distributor.Distribute(value, target, new WeightRangeMapping(minWeight, maxWeight));

// have your own user-defined type implement IWeightDistributionPolicy for custom behavior!
```

### Authors

Sky4ce09

### Links

GitHub: https://github.com/Sky4ce09/Randomness
using System.Numerics;

namespace Randomness.Distributing;

public interface IValueDistributor
{
    public void Distribute<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>;
    public void DistributeApproximate<T>(T value, Span<double> weights, Span<T> target) where T : unmanaged, INumber<T>;
}

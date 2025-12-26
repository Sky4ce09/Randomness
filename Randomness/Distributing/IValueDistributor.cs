using System.Runtime.InteropServices;

namespace Randomness.Distributing;

public interface IValueDistributor
{
    public void Distribute(int value, Span<double> weights, Span<int> target);
    public void DistributeApproximate(int value, Span<double> weights, Span<int> target);
    public void Distribute(long value, Span<double> weights, Span<long> target);
    public void DistributeApproximate(long value, Span<double> weights, Span<long> target);
    public void Distribute(nint value, Span<double> weights, Span<nint> target);
    public void DistributeApproximate(nint value, Span<double> weights, Span<nint> target);
    public void Distribute(float value, Span<double> weights, Span<float> target);
    public void Distribute(double value, Span<double> weights, Span<double> target);
    public void Distribute(decimal value, Span<double> weights, Span<decimal> target);
    public void Distribute(NFloat value, Span<double> weights, Span<NFloat> target);
}

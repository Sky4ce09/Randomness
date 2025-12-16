namespace Randomness.Distributing;

public interface IWeightDistributionPolicy
{
    /// <summary>
    /// Modifies the provided weights.
    /// </summary>
    /// <param name="weights">The individual weights to be modified.</param>
    public void ApplyTo(Span<double> weights);
}

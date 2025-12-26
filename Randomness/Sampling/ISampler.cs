using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomness.Sampling;

public interface ISampler
{
    /// <summary>
    /// Adds a layer of sampling to the provided span.
    /// </summary>
    /// <param name="outValues"></param>
    /// <returns>The sum of values added to the span.</returns>
    public double? AddSample(Span<double> outValues);
}

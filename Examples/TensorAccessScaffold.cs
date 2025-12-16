using System.Runtime.CompilerServices;

namespace Randomness.Distributing;

/// <summary>
/// Builds tensor access behavior on top of a span. (row-major by default)
/// </summary>
/// <typeparam name="T">The type of data that is read and written.</typeparam>
public readonly ref struct TensorAccessScaffold<T>
{
    private ReadOnlySpan<int> Dimensions { get; }
    public Span<T> Span { get; }
    public int Rank { get; }
    public int[] Strides { get; }
    public bool IsRowMajor { get; }
    public int Length => Span.Length;

    public TensorAccessScaffold(Span<T> span, Span<int> dimensions, bool rowMajor = true)
    {
        Span = span;
        Rank = dimensions.Length;
        Dimensions = dimensions;
        Strides = new int[Rank];
        int strideSize = 1;
        if (rowMajor)
        {
            for (int i = Rank - 1; i >= 0; i--)
            {
                Strides[i] = strideSize;
                strideSize *= Dimensions[i];
            }
        }
        else
        {
            for (int i = 0; i < Rank; i++)
            {
                Strides[i] = strideSize;
                strideSize *= Dimensions[i];
            }
        }
        IsRowMajor = rowMajor;
    }

    public readonly T this[int n]
    {
        get => Span[n];
        set => Span[n] = value;
    }
    public readonly T this[Span<int> indices]
    {
        get
        {
            return Span[ToLinearIndex(indices)];
        }
        set
        {
            Span[ToLinearIndex(indices)] = value;
        }
    }
    public readonly T this[params int[] indices]
    {
        get
        {
            return Span[ToLinearIndex(indices)];
        }
        set
        {
            Span[ToLinearIndex(indices)] = value;
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly int ToLinearIndex(Span<int> indices)
    {
        if (indices.Length < Rank) throw new IndexOutOfRangeException("Insufficient indices provided for array access.");
        int index = 0;
        for (int i = 0; i < Rank; i++)
        {
            index += indices[i] * Strides[i];
        }
        return index;
    }
}

using System.Collections;

namespace Protobuf.NET.Wrappers;

/// <summary>
/// Base class for packed repeated field wrappers
/// </summary>
/// <typeparam name="T">The type of elements in the collection</typeparam>
public abstract class PackedRepeatedField<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> _values;

    /// <summary>
    /// Initializes a new instance of the PackedRepeatedField class
    /// </summary>
    /// <param name="values">The values to be encoded as a packed repeated field</param>
    protected PackedRepeatedField(IEnumerable<T> values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>
    /// Gets the enumerator for the collection
    /// </summary>
    public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();

    /// <summary>
    /// Gets the non-generic enumerator for the collection
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();
}
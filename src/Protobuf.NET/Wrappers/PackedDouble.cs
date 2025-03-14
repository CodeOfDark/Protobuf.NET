namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of double values
/// </summary>
public class PackedDouble : PackedRepeatedField<double>
{
    /// <summary>
    /// Initializes a new instance of the PackedDouble class
    /// </summary>
    /// <param name="values">The double values to be encoded as a packed repeated field</param>
    public PackedDouble(IEnumerable<double> values) : base(values) { }
}
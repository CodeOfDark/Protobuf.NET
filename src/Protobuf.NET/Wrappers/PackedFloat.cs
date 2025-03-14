namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of float values
/// </summary>
public class PackedFloat : PackedRepeatedField<float>
{
    /// <summary>
    /// Initializes a new instance of the PackedFloat class
    /// </summary>
    /// <param name="values">The float values to be encoded as a packed repeated field</param>
    public PackedFloat(IEnumerable<float> values) : base(values) { }
}
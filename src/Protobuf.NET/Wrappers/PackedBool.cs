namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of bool values
/// </summary>
public class PackedBool : PackedRepeatedField<bool>
{
    /// <summary>
    /// Initializes a new instance of the PackedBool class
    /// </summary>
    /// <param name="values">The bool values to be encoded as a packed repeated field</param>
    public PackedBool(IEnumerable<bool> values) : base(values) { }
}
namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of int32 values
/// </summary>
public class PackedInt32 : PackedRepeatedField<int>
{
    /// <summary>
    /// Initializes a new instance of the PackedInt32 class
    /// </summary>
    /// <param name="values">The int32 values to be encoded as a packed repeated field</param>
    public PackedInt32(IEnumerable<int> values) : base(values) { }
}
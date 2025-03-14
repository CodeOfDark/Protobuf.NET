namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of int64 values
/// </summary>
public class PackedInt64 : PackedRepeatedField<long>
{
    /// <summary>
    /// Initializes a new instance of the PackedInt64 class
    /// </summary>
    /// <param name="values">The int64 values to be encoded as a packed repeated field</param>
    public PackedInt64(IEnumerable<long> values) : base(values) { }
}
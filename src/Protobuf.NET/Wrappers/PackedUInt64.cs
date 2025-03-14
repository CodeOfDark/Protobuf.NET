namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of uint64 values
/// </summary>
public class PackedUInt64 : PackedRepeatedField<ulong>
{
    /// <summary>
    /// Initializes a new instance of the PackedUInt64 class
    /// </summary>
    /// <param name="values">The uint64 values to be encoded as a packed repeated field</param>
    public PackedUInt64(IEnumerable<ulong> values) : base(values) { }
}
namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of uint32 values
/// </summary>
public class PackedUInt32 : PackedRepeatedField<uint>
{
    /// <summary>
    /// Initializes a new instance of the PackedUInt32 class
    /// </summary>
    /// <param name="values">The uint32 values to be encoded as a packed repeated field</param>
    public PackedUInt32(IEnumerable<uint> values) : base(values) { }
}
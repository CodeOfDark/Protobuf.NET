namespace Protobuf.Core.Enums;

/// <summary>
/// Wire format types as defined in the Protocol Buffers specification
/// </summary>
public enum WireFormat
{
    /// <summary>
    /// Used for int32, int64, uint32, uint64, sint32, sint64, bool, enum
    /// </summary>
    Varint = 0,
        
    /// <summary>
    /// Used for fixed64, sfixed64, double
    /// </summary>
    Fixed64 = 1,
        
    /// <summary>
    /// Used for string, bytes, embedded messages, packed repeated fields
    /// </summary>
    LengthDelimited = 2,
        
    /// <summary>
    /// Used for fixed32, sfixed32, float
    /// </summary>
    Fixed32 = 5
}
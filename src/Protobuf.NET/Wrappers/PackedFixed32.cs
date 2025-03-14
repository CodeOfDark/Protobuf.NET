namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of fixed32 values
/// </summary>
public class PackedFixed32 : PackedRepeatedField<Fixed32>
{
    /// <summary>
    /// Initializes a new instance of the PackedFixed32 class
    /// </summary>
    /// <param name="values">The fixed32 values to be encoded as a packed repeated field</param>
    public PackedFixed32(IEnumerable<Fixed32> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedFixed32 class
    /// </summary>
    /// <param name="values">The uint values to be encoded as fixed32 in a packed repeated field</param>
    public PackedFixed32(IEnumerable<uint> values) 
        : base(ConvertToFixed32(values)) { }
    
    private static IEnumerable<Fixed32> ConvertToFixed32(IEnumerable<uint> values)
    {
        return values.Select(value => new Fixed32(value));
    }
}
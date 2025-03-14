namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of fixed64 values
/// </summary>
public class PackedFixed64 : PackedRepeatedField<Fixed64>
{
    /// <summary>
    /// Initializes a new instance of the PackedFixed64 class
    /// </summary>
    /// <param name="values">The fixed64 values to be encoded as a packed repeated field</param>
    public PackedFixed64(IEnumerable<Fixed64> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedFixed64 class
    /// </summary>
    /// <param name="values">The ulong values to be encoded as fixed64 in a packed repeated field</param>
    public PackedFixed64(IEnumerable<ulong> values) 
        : base(ConvertToFixed64(values)) { }
            
    private static IEnumerable<Fixed64> ConvertToFixed64(IEnumerable<ulong> values)
    {
        return values.Select(value => new Fixed64(value));
    }
}
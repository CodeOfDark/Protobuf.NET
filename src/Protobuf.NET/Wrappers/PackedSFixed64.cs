namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of sfixed64 values
/// </summary>
public class PackedSFixed64 : PackedRepeatedField<SFixed64>
{
    /// <summary>
    /// Initializes a new instance of the PackedSFixed64 class
    /// </summary>
    /// <param name="values">The sfixed64 values to be encoded as a packed repeated field</param>
    public PackedSFixed64(IEnumerable<SFixed64> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedSFixed64 class
    /// </summary>
    /// <param name="values">The long values to be encoded as sfixed64 in a packed repeated field</param>
    public PackedSFixed64(IEnumerable<long> values) 
        : base(ConvertToSFixed64(values)) { }
            
    private static IEnumerable<SFixed64> ConvertToSFixed64(IEnumerable<long> values)
    {
        return values.Select(value => new SFixed64(value));
    }
}
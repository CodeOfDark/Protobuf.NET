namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of sfixed32 values
/// </summary>
public class PackedSFixed32 : PackedRepeatedField<SFixed32>
{
    /// <summary>
    /// Initializes a new instance of the PackedSFixed32 class
    /// </summary>
    /// <param name="values">The sfixed32 values to be encoded as a packed repeated field</param>
    public PackedSFixed32(IEnumerable<SFixed32> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedSFixed32 class
    /// </summary>
    /// <param name="values">The int values to be encoded as sfixed32 in a packed repeated field</param>
    public PackedSFixed32(IEnumerable<int> values) 
        : base(ConvertToSFixed32(values)) { }
            
    private static IEnumerable<SFixed32> ConvertToSFixed32(IEnumerable<int> values)
    {
        return values.Select(value => new SFixed32(value));
    }
}
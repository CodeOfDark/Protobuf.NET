namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of sint32 values
/// </summary>
public class PackedSInt32 : PackedRepeatedField<SInt32>
{
    /// <summary>
    /// Initializes a new instance of the PackedSInt32 class
    /// </summary>
    /// <param name="values">The sint32 values to be encoded as a packed repeated field</param>
    public PackedSInt32(IEnumerable<SInt32> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedSInt32 class
    /// </summary>
    /// <param name="values">The int values to be encoded as sint32 in a packed repeated field</param>
    public PackedSInt32(IEnumerable<int> values) 
        : base(ConvertToSInt32(values)) { }
            
    private static IEnumerable<SInt32> ConvertToSInt32(IEnumerable<int> values)
    {
        return values.Select(value => new SInt32(value));
    }
}
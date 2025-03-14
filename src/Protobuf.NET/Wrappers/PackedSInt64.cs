namespace Protobuf.NET.Wrappers;

/// <summary>
/// Represents a packed repeated field of sint64 values
/// </summary>
public class PackedSInt64 : PackedRepeatedField<SInt64>
{
    /// <summary>
    /// Initializes a new instance of the PackedSInt64 class
    /// </summary>
    /// <param name="values">The sint64 values to be encoded as a packed repeated field</param>
    public PackedSInt64(IEnumerable<SInt64> values) : base(values) { }
        
    /// <summary>
    /// Initializes a new instance of the PackedSInt64 class
    /// </summary>
    /// <param name="values">The long values to be encoded as sint64 in a packed repeated field</param>
    public PackedSInt64(IEnumerable<long> values) 
        : base(ConvertToSInt64(values)) { }
            
    private static IEnumerable<SInt64> ConvertToSInt64(IEnumerable<long> values)
    {
        return values.Select(value => new SInt64(value));
    }
}
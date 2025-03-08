namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a range of field numbers in a reserved statement.
/// </summary>
public class ProtoRange
{
    /// <summary>
    /// Gets or sets the start of the range.
    /// </summary>
    public int From { get; set; }

    /// <summary>
    /// Gets or sets the end of the range.
    /// </summary>
    public int To { get; set; }
        
    /// <summary>
    /// Gets whether this range represents a single number.
    /// </summary>
    public bool IsSingleNumber => From == To;
        
    /// <summary>
    /// Returns a string representation of the range.
    /// </summary>
    public override string ToString() => IsSingleNumber ? From.ToString() : $"{From} to {To}";
}
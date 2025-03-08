namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a reserved field range or name in a Protocol Buffer message.
/// </summary>
public class ProtoReserved
{
    /// <summary>
    /// Gets or sets whether this reserved statement refers to field names.
    /// </summary>
    public bool IsName { get; set; }

    /// <summary>
    /// Gets or sets the list of reserved field names.
    /// </summary>
    public List<string> FieldNames { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of reserved field number ranges.
    /// </summary>
    public List<ProtoRange> Ranges { get; set; } = [];
}
namespace Protobuf.Parser.Models;

/// <summary>
/// Represents an extension in a Protocol Buffer file.
/// </summary>
public class ProtoExtension
{
    /// <summary>
    /// Gets or sets the type being extended.
    /// </summary>
    public string ExtendedType { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of extension fields.
    /// </summary>
    public List<ProtoField> Fields { get; set; } = [];
}
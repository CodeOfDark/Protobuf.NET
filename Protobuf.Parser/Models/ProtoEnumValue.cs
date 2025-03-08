namespace Protobuf.Parser.Models;

/// <summary>
/// Represents an enum value in a Protocol Buffer enum.
/// </summary>
public class ProtoEnumValue
{
    /// <summary>
    /// Gets or sets the enum value name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the enum value number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the list of options for this enum value.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];
}
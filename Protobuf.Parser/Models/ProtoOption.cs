namespace Protobuf.Parser.Models;

/// <summary>
/// Represents an option in a Protocol Buffer file.
/// </summary>
public class ProtoOption
{
    /// <summary>
    /// Gets or sets the option name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    public string Value { get; set; } = "";
        
    /// <summary>
    /// Gets whether this is a custom option.
    /// </summary>
    public bool IsCustom => Name.StartsWith('(') && Name.EndsWith(')');
}
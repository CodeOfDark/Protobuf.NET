namespace Protobuf.Parser.Models;

/// <summary>
/// Represents an enum in a Protocol Buffer file.
/// </summary>
public class ProtoEnum
{
    /// <summary>
    /// Gets or sets the enum name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the parent message name (for nested enums).
    /// </summary>
    public string? ParentMessage { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of enum values.
    /// </summary>
    public List<ProtoEnumValue> Values { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of enum options.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];
        
    /// <summary>
    /// Gets the full name of the enum, including parent message names.
    /// </summary>
    public string FullName => string.IsNullOrEmpty(ParentMessage) ? Name : $"{ParentMessage}.{Name}";

    /// <summary>
    /// Gets the enum value with the specified name.
    /// </summary>
    /// <param name="name">The name of the enum value to get.</param>
    /// <returns>The enum value with the specified name, or <c>null</c> if no such value exists.</returns>
    public ProtoEnumValue? GetValue(string name) => Values.FirstOrDefault(value => value.Name == name);
}
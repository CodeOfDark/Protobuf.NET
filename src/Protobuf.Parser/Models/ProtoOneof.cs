namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a oneof in a Protocol Buffer message.
/// </summary>
public class ProtoOneof
{
    /// <summary>
    /// Gets or sets the oneof name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of fields in the oneof.
    /// </summary>
    public List<ProtoField> Fields { get; set; } = [];
        
    /// <summary>
    /// Gets or sets the parent message.
    /// </summary>
    public ProtoMessage? ParentMessage { get; set; }
        
    /// <summary>
    /// Gets the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field to get.</param>
    /// <returns>The field with the specified name, or <c>null</c> if no such field exists.</returns>
    public ProtoField? GetField(string name) => Fields.FirstOrDefault(field => field.Name == name);
}
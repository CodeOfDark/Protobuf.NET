namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a message in a Protocol Buffer file.
/// </summary>
public class ProtoMessage
{
    /// <summary>
    /// Gets or sets the message name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the parent message name (for nested messages).
    /// </summary>
    public string? ParentMessage { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of fields in the message.
    /// </summary>
    public List<ProtoField> Fields { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of options for the message.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of nested messages within this message.
    /// </summary>
    public List<ProtoMessage> NestedMessages { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of nested enums within this message.
    /// </summary>
    public List<ProtoEnum> NestedEnums { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of oneofs within this message.
    /// </summary>
    public List<ProtoOneof> Oneofs { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of extensions defined in this message.
    /// </summary>
    public List<ProtoExtension> Extensions { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of reserved fields or names.
    /// </summary>
    public List<ProtoReserved> Reserved { get; set; } = [];
        
    /// <summary>
    /// Gets the full name of the message, including parent message names.
    /// </summary>
    public string FullName => string.IsNullOrEmpty(ParentMessage) ? Name : $"{ParentMessage}.{Name}";

    /// <summary>
    /// Gets the list of all fields, including those in oneofs.
    /// </summary>
    public IEnumerable<ProtoField> AllFields
    {
        get
        {
            foreach (var field in Fields)
            {
                yield return field;
            }

            foreach (var field in Oneofs.SelectMany(oneof => oneof.Fields))
            {
                yield return field;
            }
        }
    }
        
    /// <summary>
    /// Gets the field with the specified name.
    /// </summary>
    /// <param name="name">The name of the field to get.</param>
    /// <returns>The field with the specified name, or <c>null</c> if no such field exists.</returns>
    public ProtoField? GetField(string name) => AllFields.FirstOrDefault(field => field.Name == name);
}
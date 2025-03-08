namespace Protobuf.Parser.Validation;

/// <summary>
/// Information about a type.
/// </summary>
public class TypeInfo
{
    /// <summary>
    /// Gets or sets the type name.
    /// </summary>
    public required string Name { get; set; }
        
    /// <summary>
    /// Gets or sets the kind of type.
    /// </summary>
    public TypeKind Kind { get; set; }
}
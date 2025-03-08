namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a field in a Protocol Buffer message.
/// </summary>
public class ProtoField
{
    /// <summary>
    /// Gets or sets the field rule (optional, required, repeated).
    /// </summary>
    public string Rule { get; set; } = "";

    /// <summary>
    /// Gets or sets the field type.
    /// </summary>
    public string Type { get; set; } = "";

    /// <summary>
    /// Gets or sets the generic type for parameterized types.
    /// </summary>
    public string GenericType { get; set; } = "";

    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the field number.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the list of field options.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];

    /// <summary>
    /// Gets or sets whether this is a map field.
    /// </summary>
    public bool IsMap { get; set; }

    /// <summary>
    /// Gets or sets the key type for map fields.
    /// </summary>
    public string KeyType { get; set; } = "";

    /// <summary>
    /// Gets or sets whether this field is part of a oneof.
    /// </summary>
    public bool IsOneofField { get; set; }
        
    /// <summary>
    /// Gets or sets the oneof this field belongs to.
    /// </summary>
    public ProtoOneof? Oneof { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedType.
    /// </summary>
    public object? ResolvedType { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedFile.
    /// </summary>
    public ProtoFile? ResolvedFile { get; set; }
        
    /// <summary>
    /// Gets whether this field is repeated.
    /// </summary>
    public bool IsRepeated => Rule == "repeated";
        
    /// <summary>
    /// Gets whether this field is required.
    /// </summary>
    public bool IsRequired => Rule == "required";
        
    /// <summary>
    /// Gets whether this field is optional.
    /// </summary>
    public bool IsOptional => Rule == "optional";
        
    /// <summary>
    /// Gets the effective type of the field, including generic types.
    /// </summary>
    public string EffectiveType
    {
        get
        {
            if (IsMap)
                return $"map<{KeyType}, {Type}>";

            return !string.IsNullOrEmpty(GenericType) ? $"{Type}<{GenericType}>" : Type;
        }
    }
        
    /// <summary>
    /// Gets whether this field is a primitive type.
    /// </summary>
    public bool IsPrimitive
    {
        get
        {
            return Type.ToLower() switch
            {
                "double" or "float" or "int32" or "int64" or "uint32" or "uint64" or "sint32" or "sint64"
                    or "fixed32" or "fixed64" or "sfixed32" or "sfixed64" or "bool" or "string" or "bytes" => true,
                _ => false
            };
        }
    }
}
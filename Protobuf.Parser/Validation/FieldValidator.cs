using Protobuf.Parser.Models;

namespace Protobuf.Parser.Validation;

/// <summary>
/// Validates Protocol Buffer field definitions.
/// </summary>
public class FieldValidator
{
    /// <summary>
    /// Validates a field definition.
    /// </summary>
    /// <param name="field">The field to validate.</param>
    /// <param name="syntax">The Protocol Buffer syntax version.</param>
    /// <returns>A collection of validation errors, if any.</returns>
    public static IEnumerable<string> ValidateField(ProtoField field, string syntax)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(field.Name))
            errors.Add("Field name cannot be empty.");
        else if (!IsValidIdentifier(field.Name))
            errors.Add($"Invalid field name '{field.Name}'.");
        
        if (string.IsNullOrEmpty(field.Type))
            errors.Add($"Type for field '{field.Name}' cannot be empty.");
        
        if (field.Number <= 0)
            errors.Add($"Field number for field '{field.Name}' must be greater than 0.");
        else if (field.Number is >= 19000 and <= 19999)
            errors.Add($"Field number {field.Number} for field '{field.Name}' is in the reserved range [19000, 19999].");
        
        if (syntax == "proto3")
        {
            if (field.IsRequired)
                errors.Add($"Field '{field.Name}' cannot be 'required' in proto3.");
            
            if (field is { IsOptional: true, IsOneofField: false })
                errors.Add($"Field '{field.Name}' does not need the 'optional' label in proto3.");
        }
        
        if (field.IsMap)
        {
            if (!string.IsNullOrEmpty(field.Rule))
                errors.Add($"Map field '{field.Name}' cannot have a label (optional, required, repeated).");
            
            if (string.IsNullOrEmpty(field.KeyType))
                errors.Add($"Key type for map field '{field.Name}' cannot be empty.");
            else if (!IsValidMapKeyType(field.KeyType))
                errors.Add($"Invalid key type '{field.KeyType}' for map field '{field.Name}'. Key type must be a scalar.");
        }
            
        return errors;
    }
        
    private static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;
        
        if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
            return false;
        
        for (var i = 1; i < identifier.Length; i++)
        {
            if (!char.IsLetterOrDigit(identifier[i]) && identifier[i] != '_')
                return false;
        }
            
        return true;
    }
        
    private static bool IsValidMapKeyType(string keyType)
    {
        return keyType switch
        {
            "int32" or "int64" or "uint32" or "uint64" or "sint32" or "sint64" or "fixed32" or "fixed64" or "sfixed32"
                or "sfixed64" or "bool" or "string" => true,
            _ => false
        };
    }
}
using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;

namespace Protobuf.Parser.Validation;

/// <summary>
/// Validates Protocol Buffer definitions.
/// </summary>
public class ProtoValidator
{
    private readonly FieldValidator _fieldValidator;
    private readonly ReferenceValidator _referenceValidator;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoValidator"/> class.
    /// </summary>
    public ProtoValidator()
    {
        _fieldValidator = new FieldValidator();
        _referenceValidator = new ReferenceValidator();
    }
        
    /// <summary>
    /// Validates a Protocol Buffer file.
    /// </summary>
    /// <param name="protoFile">The Protocol Buffer file to validate.</param>
    /// <exception cref="ProtoValidationException">Thrown if validation fails.</exception>
    public void Validate(ProtoFile protoFile)
    {
        var errors = new List<string>();
        
        if (protoFile.Syntax != "proto2" && protoFile.Syntax != "proto3")
            errors.Add($"Invalid syntax '{protoFile.Syntax}'. Must be 'proto2' or 'proto3'.");
        
        if (!string.IsNullOrEmpty(protoFile.Package) && !IsValidPackageName(protoFile.Package))
            errors.Add($"Invalid package name '{protoFile.Package}'.");

        errors.AddRange(from import in protoFile.Imports where string.IsNullOrEmpty(import.Path) select "Import path cannot be empty.");

        foreach (var message in protoFile.Messages)
            errors.AddRange(ValidateMessage(message, protoFile));
            
        foreach (var protoEnum in protoFile.Enums)
            errors.AddRange(ValidateEnum(protoEnum));
        
        foreach (var service in protoFile.Services)
            errors.AddRange(ValidateService(service));
        
        try
        {
            _referenceValidator.ValidateReferences(protoFile);
        }
        catch (ProtoValidationException ex)
        {
            errors.AddRange(ex.ValidationErrors);
        }
        
        if (errors.Count > 0)
            throw new ProtoValidationException(errors);
    }
        
    /// <summary>
    /// Validates a collection of Protocol Buffer files.
    /// </summary>
    /// <param name="protoFiles">The Protocol Buffer files to validate.</param>
    /// <exception cref="ProtoValidationException">Thrown if validation fails.</exception>
    public void ValidateAll(IEnumerable<ProtoFile> protoFiles)
    {
        var errors = new List<string>();
        var filesList = protoFiles.ToList();
        
        foreach (var protoFile in filesList)
        {
            try
            {
                Validate(protoFile);
            }
            catch (ProtoValidationException ex)
            {
                errors.Add($"Validation errors in file '{protoFile.FileName}':");
                errors.AddRange(ex.ValidationErrors.Select(error => $"  - {error}"));
            }
        }
        
        try
        {
            _referenceValidator.ValidateReferences(filesList);
        }
        catch (ProtoValidationException ex)
        {
            errors.AddRange(ex.ValidationErrors);
        }
        
        if (errors.Count > 0)
            throw new ProtoValidationException(errors);
    }
        
    private IEnumerable<string> ValidateMessage(ProtoMessage message, ProtoFile protoFile)
    {
        var errors = new List<string>();
        if (string.IsNullOrEmpty(message.Name))
            errors.Add("Message name cannot be empty.");
        else if (!IsValidIdentifier(message.Name))
            errors.Add($"Invalid message name '{message.Name}'.");
        
        foreach (var field in message.Fields)
            errors.AddRange(FieldValidator.ValidateField(field, protoFile.Syntax));
        
        var fieldNumbers = new HashSet<int>();
        errors.AddRange(from field in message.AllFields where !fieldNumbers.Add(field.Number) select $"Duplicate field number '{field.Number}' in message '{message.Name}'.");

        foreach (var nestedMessage in message.NestedMessages)
            errors.AddRange(ValidateMessage(nestedMessage, protoFile));
        
        foreach (var nestedEnum in message.NestedEnums)
            errors.AddRange(ValidateEnum(nestedEnum));
        
        foreach (var oneof in message.Oneofs)
        {
            if (string.IsNullOrEmpty(oneof.Name))
                errors.Add("Oneof name cannot be empty.");
            else if (!IsValidIdentifier(oneof.Name))
                errors.Add($"Invalid oneof name '{oneof.Name}'.");
            
            if (oneof.Fields.Count == 0)
                errors.Add($"Oneof '{oneof.Name}' must contain at least one field.");
                
            foreach (var field in oneof.Fields)
            {
                errors.AddRange(FieldValidator.ValidateField(field, protoFile.Syntax));
                
                if (field.IsRepeated)
                    errors.Add($"Field '{field.Name}' in oneof '{oneof.Name}' cannot be repeated.");
            }
        }
            
        return errors;
    }
        
    private IEnumerable<string> ValidateEnum(ProtoEnum protoEnum)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrEmpty(protoEnum.Name))
            errors.Add("Enum name cannot be empty.");
        else if (!IsValidIdentifier(protoEnum.Name))
            errors.Add($"Invalid enum name '{protoEnum.Name}'.");
        
        if (protoEnum.Values.Count == 0)
            errors.Add($"Enum '{protoEnum.Name}' must contain at least one value.");
        
        var valueNames = new HashSet<string>();
        var valueNumbers = new HashSet<int>();
            
        foreach (var value in protoEnum.Values)
        {
            if (string.IsNullOrEmpty(value.Name))
                errors.Add("Enum value name cannot be empty.");
            else if (!IsValidIdentifier(value.Name))
                errors.Add($"Invalid enum value name '{value.Name}'.");
            else if (!valueNames.Add(value.Name))
                errors.Add($"Duplicate enum value name '{value.Name}' in enum '{protoEnum.Name}'.");

            if (!valueNumbers.Add(value.Number))
                errors.Add($"Duplicate enum value number '{value.Number}' in enum '{protoEnum.Name}'.");
        }
            
        return errors;
    }
        
    private IEnumerable<string> ValidateService(ProtoService service)
    {
        var errors = new List<string>();
            
        if (string.IsNullOrEmpty(service.Name))
            errors.Add("Service name cannot be empty.");
        else if (!IsValidIdentifier(service.Name))
            errors.Add($"Invalid service name '{service.Name}'.");
            
        var methodNames = new HashSet<string>();
        foreach (var method in service.Methods)
        {
            if (string.IsNullOrEmpty(method.Name))
                errors.Add("Method name cannot be empty.");
            else if (!IsValidIdentifier(method.Name))
                errors.Add($"Invalid method name '{method.Name}'.");
            else if (!methodNames.Add(method.Name))
                errors.Add($"Duplicate method name '{method.Name}' in service '{service.Name}'.");

            if (string.IsNullOrEmpty(method.InputType))
                errors.Add($"Input type for method '{method.Name}' cannot be empty.");
                
            if (string.IsNullOrEmpty(method.OutputType))
                errors.Add($"Output type for method '{method.Name}' cannot be empty.");
        }
            
        return errors;
    }
        
    private bool IsValidIdentifier(string identifier)
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
        
    private bool IsValidPackageName(string packageName)
    {
        if (string.IsNullOrEmpty(packageName))
            return false;
        
        var parts = packageName.Split('.');
        return parts.All(IsValidIdentifier);
    }
}
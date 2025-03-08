using System.Reflection;
using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;

namespace Protobuf.Parser.Validation;

/// <summary>
/// Validates references between Protocol Buffer elements.
/// </summary>
public class ReferenceValidator
{
    /// <summary>
    /// Validates references within a Protocol Buffer file.
    /// </summary>
    /// <param name="protoFile">The Protocol Buffer file to validate.</param>
    /// <exception cref="ProtoValidationException">Thrown if validation fails.</exception>
    public void ValidateReferences(ProtoFile protoFile)
    {
        var errors = new List<string>();
        var typeMap = BuildTypeMap([protoFile]);
        
        foreach (var message in protoFile.Messages)
            errors.AddRange(ValidateMessageReferences(message, typeMap, protoFile.Package));
        
        foreach (var service in protoFile.Services)
            errors.AddRange(ValidateServiceReferences(service, typeMap, protoFile.Package));
        
        foreach (var extension in protoFile.Extensions)
            errors.AddRange(ValidateExtensionReferences(extension, typeMap, protoFile.Package));
        
        if (errors.Count > 0)
            throw new ProtoValidationException(errors);
    }
        
    /// <summary>
    /// Validates references between Protocol Buffer files.
    /// </summary>
    /// <param name="protoFiles">The Protocol Buffer files to validate.</param>
    /// <exception cref="ProtoValidationException">Thrown if validation fails.</exception>
    public void ValidateReferences(IEnumerable<ProtoFile> protoFiles)
    {
        var errors = new List<string>();
        var filesList = protoFiles.ToList();
        var typeMap = BuildTypeMap(filesList);
        
        foreach (var protoFile in filesList)
        {
            foreach (var message in protoFile.Messages)
                errors.AddRange(ValidateMessageReferences(message, typeMap, protoFile.Package));
            
            foreach (var service in protoFile.Services)
                errors.AddRange(ValidateServiceReferences(service, typeMap, protoFile.Package));
            
            foreach (var extension in protoFile.Extensions)
                errors.AddRange(ValidateExtensionReferences(extension, typeMap, protoFile.Package));
        }
        
        if (errors.Count > 0)
            throw new ProtoValidationException(errors);
    }
        
    private IEnumerable<string> ValidateMessageReferences(ProtoMessage message, Dictionary<string, TypeInfo> typeMap, string currentPackage)
    {
        var errors = new List<string>();
        foreach (var field in message.AllFields)
        {
            if (field.IsPrimitive)
                continue;
            
            var fullTypeName = ResolveTypeName(field.Type, currentPackage);
            if (!typeMap.ContainsKey(fullTypeName))
                errors.Add($"Unknown type '{field.Type}' for field '{field.Name}' in message '{message.FullName}'.");
        }
        
        foreach (var nestedMessage in message.NestedMessages)
            errors.AddRange(ValidateMessageReferences(nestedMessage, typeMap, currentPackage));
        
        return errors;
    }
        
    private IEnumerable<string> ValidateServiceReferences(ProtoService service, Dictionary<string, TypeInfo> typeMap, string currentPackage)
    {
        var errors = new List<string>();
        foreach (var method in service.Methods)
        {
            var fullInputType = ResolveTypeName(method.InputType, currentPackage);
            if (!typeMap.ContainsKey(fullInputType))
                errors.Add($"Unknown input type '{method.InputType}' for method '{method.Name}' in service '{service.Name}'.");
            
            var fullOutputType = ResolveTypeName(method.OutputType, currentPackage);
            if (!typeMap.ContainsKey(fullOutputType))
                errors.Add($"Unknown output type '{method.OutputType}' for method '{method.Name}' in service '{service.Name}'.");
        }
            
        return errors;
    }
        
    private IEnumerable<string> ValidateExtensionReferences(ProtoExtension extension, Dictionary<string, TypeInfo> typeMap, string currentPackage)
    {
        var errors = new List<string>();
        var fullExtendedType = ResolveTypeName(extension.ExtendedType, currentPackage);
        if (!typeMap.TryGetValue(fullExtendedType, out var typeInfo))
        {
            errors.Add($"Unknown extended type '{extension.ExtendedType}'.");
        }
        else
        {
            if (typeInfo.Kind != TypeKind.Message)
                errors.Add($"Extended type '{extension.ExtendedType}' must be a message.");
        }
            
        return errors;
    }
        
    private Dictionary<string, TypeInfo> BuildTypeMap(IEnumerable<ProtoFile> protoFiles)
    {
        var typeMap = new Dictionary<string, TypeInfo>();
        foreach (var protoFile in protoFiles)
        {
            foreach (var message in protoFile.Messages)
                AddMessageToTypeMap(message, protoFile.Package, typeMap);
            
            foreach (var protoEnum in protoFile.Enums)
                AddEnumToTypeMap(protoEnum, protoFile.Package, typeMap);
        }
            
        return typeMap;
    }
        
    private void AddMessageToTypeMap(ProtoMessage message, string packageName, Dictionary<string, TypeInfo> typeMap)
    {
        var fullName = message.FullName;
        if (!string.IsNullOrEmpty(packageName) && string.IsNullOrEmpty(message.ParentMessage))
            fullName = $"{packageName}.{fullName}";
        
        typeMap[fullName] = new TypeInfo
        {
            Name = fullName,
            Kind = TypeKind.Message
        };
        
        foreach (var nestedMessage in message.NestedMessages)
            AddMessageToTypeMap(nestedMessage, packageName, typeMap);
        
        foreach (var nestedEnum in message.NestedEnums)
            AddEnumToTypeMap(nestedEnum, packageName, typeMap);
    }
        
    private void AddEnumToTypeMap(ProtoEnum protoEnum, string packageName, Dictionary<string, TypeInfo> typeMap)
    {
        var fullName = protoEnum.FullName;
        if (!string.IsNullOrEmpty(packageName) && string.IsNullOrEmpty(protoEnum.ParentMessage))
            fullName = $"{packageName}.{fullName}";
        
        typeMap[fullName] = new TypeInfo
        {
            Name = fullName,
            Kind = TypeKind.Enum
        };
    }

    private string ResolveTypeName(string typeName, string currentPackage)
    {
        if (typeName.StartsWith('.'))
            return typeName[1..];
        
        if (typeName.Contains('.'))
            return typeName;
        
        return !string.IsNullOrEmpty(currentPackage) ? $"{currentPackage}.{typeName}" : typeName;
    }
}
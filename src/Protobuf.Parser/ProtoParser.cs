using Protobuf.Parser.Abstractions;
using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;
using Protobuf.Parser.Utilities;

namespace Protobuf.Parser;

/// <summary>
/// Main parser for Protocol Buffer files.
/// </summary>
public class ProtoParser : ProtoParserBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParser"/> class.
    /// </summary>
    public ProtoParser() : base(new ProtoTokenizer())
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParser"/> class with a custom tokenizer.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to use.</param>
    public ProtoParser(ITokenizer tokenizer) : base(tokenizer)
    {
    }
        
    /// <inheritdoc/>
    public override ProtoFile ParseContent(string content, string fileName = "")
    {
        try
        {
            var tokens = Tokenizer.Tokenize(content).ToList();
            var commentMap = CommentExtractor.ExtractComments(tokens);
            
            var filteredTokens = tokens.Where(t => t.Type != TokenType.Comment && t.Type != TokenType.Whitespace).ToList();
            
            var tokenReader = new TokenReader(filteredTokens, fileName);
            var fileParser = new ProtoFileParser(tokenReader);
            
            var protoFile = fileParser.Parse();
            
            protoFile.Comments = commentMap;
            protoFile.RawContent = content;
            protoFile.FileName = fileName;
            
            return protoFile;
        }
        catch (Exception ex) when (ex is not ProtoParseException)
        {
            throw new ProtoParseException($"Failed to parse Protocol Buffer content{(string.IsNullOrEmpty(fileName) ? "" : $" in {fileName}")}: {ex.Message}", ex);
        }
    }
        
    /// <inheritdoc/>
    protected override void ResolveReferences(ICollection<ProtoFile> files)
    {
        var fileMap = files.ToDictionary(f => PathUtils.ResolvePath(f.FilePath) , f => f);
        var packageMap = new Dictionary<string, List<ProtoFile>>();
        var typeRegistry = new Dictionary<string, (ProtoFile File, object Type)>();
        
        foreach (var file in files)
        {
            if (string.IsNullOrEmpty(file.Package)) 
                continue;
            
            if (!packageMap.TryGetValue(file.Package, out var value))
            {
                value = [];
                packageMap[file.Package] = value;
            }

            value.Add(file);
        }
        
        foreach (var file in files)
        {
            foreach (var import in file.Imports)
            {
                var importPath = PathUtils.ResolveImportPath(file.FilePath, import.Path);
                if (fileMap.TryGetValue(importPath, out var importedFile))
                    import.ResolvedFile = importedFile;
            }
        }
        
        foreach (var file in files)
        {
            var packagePrefix = string.IsNullOrEmpty(file.Package) ? "" : file.Package + ".";
            foreach (var message in file.Messages)
            {
                var fullName = packagePrefix + message.Name;
                typeRegistry[fullName] = (file, message);
                RegisterNestedTypes(message, fullName, file, typeRegistry);
            }
            
            foreach (var protoEnum in file.Enums)
            {
                var fullName = packagePrefix + protoEnum.Name;
                typeRegistry[fullName] = (file, protoEnum);
            }
        }
        
        foreach (var file in files)
        {
            var packagePrefix = string.IsNullOrEmpty(file.Package) ? "" : file.Package + ".";
            foreach (var message in file.Messages)
                ResolveMessageReferences(message, file, packagePrefix, typeRegistry);
            
            foreach (var service in file.Services)
                ResolveServiceReferences(service, file, packagePrefix, typeRegistry);
            
            foreach (var extension in file.Extensions)
                ResolveExtensionReferences(extension, file, packagePrefix, typeRegistry);
        }
    }

    private static void RegisterNestedTypes(ProtoMessage message, string parentFullName, ProtoFile file, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        foreach (var nestedMessage in message.NestedMessages)
        {
            var fullName = $"{parentFullName}.{nestedMessage.Name}";
            typeRegistry[fullName] = (file, nestedMessage);
            RegisterNestedTypes(nestedMessage, fullName, file, typeRegistry);
        }
        
        foreach (var nestedEnum in message.NestedEnums)
        {
            var fullName = $"{parentFullName}.{nestedEnum.Name}";
            typeRegistry[fullName] = (file, nestedEnum);
        }
    }

    private void ResolveMessageReferences(ProtoMessage message, ProtoFile file, string packagePrefix, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        foreach (var field in message.Fields.Where(field => !field.IsPrimitive))
            ResolveTypeReference(field, file, packagePrefix, typeRegistry);
        
        foreach (var nestedMessage in message.NestedMessages)
            ResolveMessageReferences(nestedMessage, file, packagePrefix, typeRegistry);
        
        foreach (var field in message.Oneofs.SelectMany(oneof => oneof.Fields.Where(field => !field.IsPrimitive)))
            ResolveTypeReference(field, file, packagePrefix, typeRegistry);
    }

    private void ResolveTypeReference(ProtoField field, ProtoFile file, string packagePrefix, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        var possibleReferences = GetPossibleTypeReferences(field.Type, packagePrefix);
        foreach (var typeName in possibleReferences)
        {
            if (typeRegistry.TryGetValue(typeName, out var typeInfo))
            {
                field.ResolvedType = typeInfo.Type;
                field.ResolvedFile = typeInfo.File;
                return;
            }
        }
        
        foreach (var import in file.Imports)
        {
            if (import.ResolvedFile == null)
                continue;
                
            var importedPackagePrefix = string.IsNullOrEmpty(import.ResolvedFile.Package) ? "" : import.ResolvedFile.Package + ".";
            possibleReferences = GetPossibleTypeReferences(field.Type, importedPackagePrefix);
            
            foreach (var typeName in possibleReferences)
            {
                if (typeRegistry.TryGetValue(typeName, out var typeInfo))
                {
                    field.ResolvedType = typeInfo.Type;
                    field.ResolvedFile = typeInfo.File;
                    return;
                }
            }
        }
    }

    private void ResolveServiceReferences(ProtoService service, ProtoFile file, string packagePrefix, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        foreach (var method in service.Methods)
        {
            ResolveMethodTypeReference(method, true, file, packagePrefix, typeRegistry);
            ResolveMethodTypeReference(method, false, file, packagePrefix, typeRegistry);
        }
    }

    private void ResolveMethodTypeReference(ProtoMethod method, bool isInput, ProtoFile file, string packagePrefix, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        var typeName = isInput ? method.InputType : method.OutputType;
        var possibleReferences = GetPossibleTypeReferences(typeName, packagePrefix);
        
        foreach (var reference in possibleReferences)
        {
            if (typeRegistry.TryGetValue(reference, out var typeInfo))
            {
                if (isInput)
                {
                    method.ResolvedInputType = typeInfo.Type;
                    method.ResolvedInputFile = typeInfo.File;
                }
                else
                {
                    method.ResolvedOutputType = typeInfo.Type;
                    method.ResolvedOutputFile = typeInfo.File;
                }
                return;
            }
        }
        
        foreach (var import in file.Imports)
        {
            if (import.ResolvedFile == null)
                continue;
                
            var importedPackagePrefix = string.IsNullOrEmpty(import.ResolvedFile.Package) ? "" : import.ResolvedFile.Package + ".";
            possibleReferences = GetPossibleTypeReferences(typeName, importedPackagePrefix);
            foreach (var reference in possibleReferences)
            {
                if (typeRegistry.TryGetValue(reference, out var typeInfo))
                {
                    if (isInput)
                    {
                        method.ResolvedInputType = typeInfo.Type;
                        method.ResolvedInputFile = typeInfo.File;
                    }
                    else
                    {
                        method.ResolvedOutputType = typeInfo.Type;
                        method.ResolvedOutputFile = typeInfo.File;
                    }
                    return;
                }
            }
        }
    }

    private void ResolveExtensionReferences(ProtoExtension extension, ProtoFile file, string packagePrefix, Dictionary<string, (ProtoFile File, object Type)> typeRegistry)
    {
        var possibleReferences = GetPossibleTypeReferences(extension.ExtendedType, packagePrefix);
        foreach (var typeName in possibleReferences)
        {
            if (typeRegistry.TryGetValue(typeName, out var typeInfo))
            {
                extension.ResolvedExtendedType = typeInfo.Type;
                extension.ResolvedFile = typeInfo.File;
                break;
            }
        }
        
        foreach (var field in extension.Fields.Where(field => !field.IsPrimitive))
            ResolveTypeReference(field, file, packagePrefix, typeRegistry);
    }

    private static List<string> GetPossibleTypeReferences(string typeName, string packagePrefix)
    {
        var references = new List<string>();
        if (typeName.StartsWith('.'))
        {
            references.Add(typeName[1..]);
            return references;
        }
        
        if (typeName.Contains('.'))
            references.Add(typeName);
        
        if (!string.IsNullOrEmpty(packagePrefix))
            references.Add(packagePrefix + typeName);
        
        references.Add(typeName);
        return references;
    }
}
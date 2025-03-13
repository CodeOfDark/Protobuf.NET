using Protobuf.Core.Interfaces;
using Protobuf.NET.Exceptions;
using Protobuf.NET.Interfaces;
using Protobuf.Parser;
using Protobuf.Parser.Models;
using Protobuf.Parser.Validation;

namespace Protobuf.NET;

/// <summary>
/// Schema-aware deserializer for Protocol Buffers messages that interprets binary data
/// based on provided .proto schema files.
/// </summary>
public class ProtobufDeserializer : IDeserializer<DynamicProtobufResult>
{
    private readonly ProtoParser _parser;
    private readonly ProtoValidator _validator;
    private readonly ICodedInputStream? _input;
    private readonly DeserializerOptions _options;
    private readonly string _protoContent;
    private readonly string _parentMessageName;
    private readonly bool _isDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufDeserializer"/> class with specified parameters.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="parentMessageName">The fully qualified name of the root message type (including package if any).</param>
    /// <param name="protoFileContent">The content of a single .proto file defining the message structure.</param>
    /// <param name="protoDirectoryContent">The path to a directory containing multiple .proto files.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> or <paramref name="parentMessageName"/> is null,
    /// or when both <paramref name="protoFileContent"/> and <paramref name="protoDirectoryContent"/> are null.
    /// </exception>
    /// <remarks>
    /// Either <paramref name="protoFileContent"/> or <paramref name="protoDirectoryContent"/> must be provided,
    /// but not both. If <paramref name="protoDirectoryContent"/> is provided, the deserializer will load all
    /// .proto files from the specified directory.
    /// </remarks>
    public ProtobufDeserializer(
        ICodedInputStream input,
        string parentMessageName,
        string? protoFileContent = null,
        string? protoDirectoryContent = null,
        DeserializerOptions? options = null)
    {
        _parser = new ProtoParser();
        _validator = new ProtoValidator();
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _options = options ?? new DeserializerOptions();
        
        if (protoFileContent is null && protoDirectoryContent is null)
            throw new ArgumentNullException($"You must provide at least one of {nameof(protoFileContent)} or {nameof(protoDirectoryContent)}.");
        
        _protoContent = (protoFileContent ?? protoDirectoryContent)!;
        _isDirectory = protoDirectoryContent is not null;
        
        _parentMessageName = parentMessageName ?? throw new ArgumentNullException(nameof(parentMessageName));
    }

    /// <summary>
    /// Deserializes Protocol Buffers data using the schema and options provided in the constructor.
    /// </summary>
    /// <param name="input">Optional secondary input stream. If null, the main input stream is used.</param>
    /// <returns>A dynamic object-based result containing the deserialized data.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no input stream was provided in the constructor and none is provided to this method.
    /// </exception>
    /// <exception cref="ProtobufDeserializationException">
    /// Thrown when deserialization fails for any reason.
    /// </exception>
    /// <remarks>
    /// This method respects the schema configuration (file or directory) specified when constructing this instance.
    /// </remarks>
    public DynamicProtobufResult Deserialize(ICodedInputStream? input = null)
    {
        var activeInput = input ?? _input ?? throw new InvalidOperationException("No input stream provided");
        
        try
        {
            return _isDirectory ? 
                DeserializeFromDirectory(_protoContent, _parentMessageName, activeInput) : 
                Deserialize(_protoContent, _parentMessageName, activeInput);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new ProtobufDeserializationException($"Failed to deserialize protobuf message using {(_isDirectory ? "directory" : "file")} schema", ex);
        }
    }
    
    /// <summary>
    /// Deserializes Protocol Buffers data using a single .proto file schema.
    /// </summary>
    /// <param name="protoFileContent">The content of the .proto file defining the message structure.</param>
    /// <param name="parentMessageFullName">The fully qualified name of the root message type (including package).</param>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <returns>A dynamic object representing the deserialized message.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any parameter is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified parent message cannot be found in the schema.
    /// </exception>
    /// <exception cref="ProtobufDeserializationException">
    /// Thrown when deserialization fails for any reason.
    /// </exception>
    public DynamicProtobufResult Deserialize(string protoFileContent, string parentMessageFullName, ICodedInputStream input)
    {
        ArgumentNullException.ThrowIfNull(protoFileContent);
        ArgumentNullException.ThrowIfNull(parentMessageFullName);
        ArgumentNullException.ThrowIfNull(input);

        try
        {
            var protoFile = ParseAndValidateProtoFile(protoFileContent);
            var messages = BuildMessageRegistry([protoFile]);
            var result = DeserializeBuffer(input);
            var parent = FindParentMessage(messages, parentMessageFullName);
            
            return DeserializeMessage(parent, result, messages);
        }
        catch (Exception ex) when (ex is not (ArgumentNullException or ArgumentException))
        {
            throw new ProtobufDeserializationException("Failed to deserialize protobuf message", ex);
        }
    }
    
    /// <summary>
    /// Deserializes Protocol Buffers data using multiple .proto files from a directory.
    /// </summary>
    /// <param name="protoDirectory">The directory containing .proto files defining the message structure.</param>
    /// <param name="parentMessageFullName">The fully qualified name of the root message type (including package).</param>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <returns>A dynamic object representing the deserialized message.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any parameter is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the specified parent message cannot be found in the schema.
    /// </exception>
    /// <exception cref="ProtobufDeserializationException">
    /// Thrown when deserialization fails for any reason.
    /// </exception>
    /// <remarks>
    /// This method loads and parses all .proto files in the specified directory before deserializing
    /// the data. It handles cross-file references and extensions.
    /// </remarks>
    public DynamicProtobufResult DeserializeFromDirectory(string protoDirectory, string parentMessageFullName, ICodedInputStream input)
    {
        ArgumentNullException.ThrowIfNull(protoDirectory);
        ArgumentNullException.ThrowIfNull(parentMessageFullName);
        ArgumentNullException.ThrowIfNull(input);

        try
        {
            var protoFiles = ParseAndValidateProtoDirectory(protoDirectory);
            var messages = BuildMessageRegistry(protoFiles);
            var result = DeserializeBuffer(input);
            var parent = FindParentMessage(messages, parentMessageFullName);
         
            return DeserializeMessage(parent, result, messages);
        }
        catch (Exception ex) when (ex is not (ArgumentNullException or ArgumentException))
        {
            throw new ProtobufDeserializationException("Failed to deserialize protobuf message from directory", ex);
        }
    }

    /// <summary>
    /// Parses and validates a single .proto file.
    /// </summary>
    /// <param name="protoFileContent">The content of the .proto file.</param>
    /// <returns>A parsed and validated protobuf file object.</returns>
    /// <exception cref="ProtobufDeserializationException">Thrown when parsing or validation fails.</exception>
    private ProtoFile ParseAndValidateProtoFile(string protoFileContent)
    {
        try
        {
            var protoFile = _parser.ParseFile(protoFileContent);
            _validator.Validate(protoFile);
            return protoFile;
        }
        catch (Exception ex)
        {
            throw new ProtobufDeserializationException("Failed to parse or validate proto file", ex);
        }
    }
    
    /// <summary>
    /// Parses and validates all .proto files in a directory.
    /// </summary>
    /// <param name="protoDirectory">The directory containing .proto files.</param>
    /// <returns>A collection of parsed and validated protobuf file objects.</returns>
    /// <exception cref="ProtobufDeserializationException">Thrown when parsing or validation fails.</exception>
    private ICollection<ProtoFile> ParseAndValidateProtoDirectory(string protoDirectory)
    {
        try
        {
            var protoFiles = _parser.ParseDirectory(protoDirectory);
            _validator.ValidateAll(protoFiles);
            return protoFiles;
        }
        catch (Exception ex)
        {
            throw new ProtobufDeserializationException($"Failed to parse or validate proto files in directory: {protoDirectory}", ex);
        }
    }
    
    /// <summary>
    /// Builds a registry of all messages defined in the provided protobuf files.
    /// </summary>
    /// <param name="protoFiles">The collection of protobuf files to process.</param>
    /// <returns>A list of all message definitions with their fully qualified names.</returns>
    private List<MessageInfo> BuildMessageRegistry(ICollection<ProtoFile> protoFiles)
    {
        var messages = new List<MessageInfo>();
        
        foreach (var protoFile in protoFiles)
            messages.AddRange(BuildMessagesFromFile(protoFile));
        
        foreach (var protoFile in protoFiles)
            ProcessExtensions(protoFile, messages);
        
        return messages;
    }

    /// <summary>
    /// Recursively collects all message definitions from a protobuf file, including nested messages.
    /// </summary>
    /// <param name="protoFile">The protobuf file to process.</param>
    /// <param name="nestedMessages">Optional list of nested messages to process instead of top-level messages.</param>
    /// <returns>A list of all message definitions from this file context.</returns>
    private List<MessageInfo> BuildMessagesFromFile(ProtoFile protoFile, List<ProtoMessage>? nestedMessages = null)
    {
        var resultMessages = new List<MessageInfo>();
        var messages = nestedMessages ?? protoFile.Messages;

        foreach (var message in messages)
        {
            if (message.NestedMessages.Count > 0)
                resultMessages.AddRange(BuildMessagesFromFile(protoFile, message.NestedMessages));

            var fullNameWithPackage = ResolveFullName(protoFile.Package, message.FullName);
            if (resultMessages.All(x => x.FullNameWithPackage != fullNameWithPackage))
            {
                resultMessages.Add(new MessageInfo
                {
                    FullNameWithPackage = fullNameWithPackage,
                    FullName = message.FullName,
                    PackageName = protoFile.Package,
                    Message = message
                });
            }
        }

        return resultMessages;
    }
    
    /// <summary>
    /// Processes extensions from a protobuf file and adds them to the target messages.
    /// </summary>
    /// <param name="protoFile">The protobuf file containing extensions.</param>
    /// <param name="messages">The registry of all messages to be updated with extensions.</param>
    /// <param name="nestedMessages">Optional list of nested messages to process instead of top-level messages.</param>
    private void ProcessExtensions(ProtoFile protoFile, List<MessageInfo> messages, List<ProtoMessage>? nestedMessages = null)
    {
        if (nestedMessages == null)
        {
            foreach (var extension in protoFile.Extensions)
            {
                var extendedTypeName = ResolveFullName(protoFile.Package, extension.ExtendedType);
                var target = messages.FirstOrDefault(m => string.Equals(m.FullNameWithPackage, extendedTypeName));
                target?.ExtraFields.AddRange(extension.Fields);
            }

            foreach (var message in protoFile.Messages)
            {
                if (message.Extensions.Count > 0)
                {
                    foreach (var extension in message.Extensions)
                    {
                        var extendedTypeName = ResolveFullName(protoFile.Package, extension.ExtendedType);
                        var target = messages.FirstOrDefault(m => string.Equals(m.FullNameWithPackage, extendedTypeName));
                        target?.ExtraFields.AddRange(extension.Fields);
                    }
                }
                
                if (message.NestedMessages.Count > 0)
                    ProcessExtensions(protoFile, messages, message.NestedMessages);
            }
        }
        else
        {
            foreach (var message in nestedMessages)
            {
                foreach (var extension in message.Extensions)
                {
                    var extendedTypeName = ResolveFullName(protoFile.Package, extension.ExtendedType);
                    var target = messages.FirstOrDefault(m => string.Equals(m.FullNameWithPackage, extendedTypeName));
                    target?.ExtraFields.AddRange(extension.Fields);
                }
            }
        }
    }

    /// <summary>
    /// Finds a message type in the registry by its fully qualified name.
    /// </summary>
    /// <param name="messages">The registry of all messages.</param>
    /// <param name="parentMessageFullName">The fully qualified name of the message to find.</param>
    /// <returns>The message information if found.</returns>
    /// <exception cref="ArgumentException">Thrown when the message is not found.</exception>
    private MessageInfo FindParentMessage(List<MessageInfo> messages, string parentMessageFullName)
    {
        var parent = messages.FirstOrDefault(t => string.Equals(t.FullNameWithPackage, parentMessageFullName));
        
        if (parent == null)
            throw new ArgumentException($"Message '{parentMessageFullName}' not found in the provided schema", nameof(parentMessageFullName));
        
        return parent;
    }

    /// <summary>
    /// Resolves a type name to its fully qualified form by prepending the package name if necessary.
    /// </summary>
    /// <param name="package">The package context.</param>
    /// <param name="typeName">The type name to resolve.</param>
    /// <returns>The fully qualified type name.</returns>
    private string ResolveFullName(string package, string typeName)
    {
        if (string.IsNullOrEmpty(package) || typeName.Contains(package))
            return typeName;
        
        return $"{package}.{typeName}";
    }
    
    /// <summary>
    /// Deserializes Protocol Buffers binary data to a dictionary-based representation.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <returns>A dictionary-based result containing the deserialized data.</returns>
    private DictionaryProtobufResult DeserializeBuffer(ICodedInputStream input)
    {
        var deserializer = new DictionaryDeserializer(input, _options);
        return deserializer.Deserialize();
    }
    
    /// <summary>
    /// Recursively deserializes a message based on its schema definition.
    /// </summary>
    /// <param name="message">The schema information for the message being deserialized.</param>
    /// <param name="dictionaryResult">The dictionary-based result containing the raw field data.</param>
    /// <param name="allMessages">The registry of all message types available in the schema.</param>
    /// <returns>A dynamic object representing the deserialized message.</returns>
    private DynamicProtobufResult DeserializeMessage(MessageInfo message, DictionaryProtobufResult dictionaryResult, List<MessageInfo> allMessages)
    {
        var result = new DynamicProtobufResult();
        var fields = new List<ProtoField>();
        fields.AddRange(message.Message.AllFields);
        fields.AddRange(message.ExtraFields);
        
        foreach (var field in fields)
        {
            if (IsScalarType(field.Type))
                DeserializeScalarField(field, dictionaryResult, result);
            else
                DeserializeMessageField(field, dictionaryResult, result, allMessages);
        }
        
        return result;
    }

    /// <summary>
    /// Deserializes a scalar (primitive) field into the result object.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="source">The source data containing the field value.</param>
    /// <param name="target">The target object to receive the deserialized value.</param>
    private void DeserializeScalarField(ProtoField field, DictionaryProtobufResult source, DynamicProtobufResult target)
    {
        if (field.IsRepeated)
        {
            var repeatedValues = source.GetRepeatedValues<object>(field.Number, field.Type);
            var objType = repeatedValues[0].GetType();
            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>) && objType.GetGenericArguments()[0] != typeof(object))
                target.Set(field.Name, repeatedValues[0]);
            else
                target.Set(field.Name, new List<object>(repeatedValues));
        }
        else
        {
            var value = source.GetValue(field.Number, field.Type);
            target.Set(field.Name, value);
        }
    }
    
    /// <summary>
    /// Deserializes a message field (complex type) into the result object.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="source">The source data containing the field value.</param>
    /// <param name="target">The target object to receive the deserialized value.</param>
    /// <param name="allMessages">The registry of all message types available in the schema.</param>
    private void DeserializeMessageField(ProtoField field, DictionaryProtobufResult source, DynamicProtobufResult target, List<MessageInfo> allMessages)
    {
        if (field.IsRepeated)
            DeserializeRepeatedMessageField(field, source, target, allMessages);
        else
            DeserializeSingleMessageField(field, source, target, allMessages);
    }

    /// <summary>
    /// Deserializes a repeated message field into the result object.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="source">The source data containing the field values.</param>
    /// <param name="target">The target object to receive the deserialized values.</param>
    /// <param name="allMessages">The registry of all message types available in the schema.</param>
    private void DeserializeRepeatedMessageField(ProtoField field, DictionaryProtobufResult source, DynamicProtobufResult target, List<MessageInfo> allMessages)
    {
        target.Set(field.Name, new List<object>());
        var repeatedFieldList = (List<object>)target.GetPropertyValue(field.Name)!;
        var repeatedValues = source.GetRepeatedValues<object>(field.Number, "message");
        foreach (var item in repeatedValues)
        {
            var fieldType = FindMessageType(field.Type, allMessages);
            if (fieldType == null)
            {
                target.Set(field.Name, source.GetValue(field.Number, "bytes"));
                continue;
            }
            
            repeatedFieldList.Add(DeserializeMessage(fieldType, (DictionaryProtobufResult)item, allMessages));
        }
    }
    
    /// <summary>
    /// Deserializes a single message field into the result object.
    /// </summary>
    /// <param name="field">The field definition.</param>
    /// <param name="source">The source data containing the field value.</param>
    /// <param name="target">The target object to receive the deserialized value.</param>
    /// <param name="allMessages">The registry of all message types available in the schema.</param>
    private void DeserializeSingleMessageField(ProtoField field, DictionaryProtobufResult source, DynamicProtobufResult target, List<MessageInfo> allMessages)
    {
        var fieldType = FindMessageType(field.Type, allMessages);
        if (fieldType == null)
        {
            target.Set(field.Name, source.GetValue(field.Number, "int32"));
            return;
        }
        
        var value = source.GetValue(field.Number, "message");
        if (value == null)
            return;
        
        target.Set(field.Name, DeserializeMessage(fieldType, (DictionaryProtobufResult)value, allMessages));
    }
    
    /// <summary>
    /// Tries to find a message type across all available schemas.
    /// </summary>
    /// <param name="typeName">The type name to find.</param>
    /// <param name="allMessages">The registry of all message types available in the schema.</param>
    /// <returns>The message information if found; otherwise, null.</returns>
    private MessageInfo? FindMessageType(string typeName, List<MessageInfo> allMessages)
    {
        foreach (var msg in allMessages)
        {
            var resolved = ResolveFullName(msg.PackageName, typeName);
            var match = allMessages.FirstOrDefault(t => string.Equals(resolved, t.FullNameWithPackage));
            
            if (match != null)
                return match;
        }
        
        return null;
    }
    
    /// <summary>
    /// Determines if a type is a scalar (primitive) Protocol Buffers type.
    /// </summary>
    /// <param name="typeName">The type name to check.</param>
    /// <returns>True if the type is a scalar type; otherwise, false.</returns>
    private static bool IsScalarType(string typeName)
    {
        return typeName switch
        {
            "int32" or "int64" or "uint32" or "uint64" or "sint32" or "sint64" or 
            "fixed32" or "fixed64" or "sfixed32" or "sfixed64" or
            "double" or "float" or 
            "bool" or "string" or "bytes" => true,
            _ => false
        };
    }
    
    /// <summary>
    /// Represents information about a protobuf message type in the schema.
    /// </summary>
    private class MessageInfo
    {
        /// <summary>
        /// Gets or sets the protobuf message definition.
        /// </summary>
        public ProtoMessage Message { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the additional fields from extensions targeting this message.
        /// </summary>
        public List<ProtoField> ExtraFields { get; set; } = [];
        
        /// <summary>
        /// Gets or sets the message's simple name (without package).
        /// </summary>
        public string FullName { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the package name.
        /// </summary>
        public string PackageName { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the fully qualified name including package.
        /// </summary>
        public string FullNameWithPackage { get; set; } = null!;
    }
}
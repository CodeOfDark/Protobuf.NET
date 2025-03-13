using Protobuf.Core;
using Protobuf.Core.Interfaces;
using Protobuf.NET.Interfaces;

namespace Protobuf.NET;

/// <summary>
/// Factory for creating Protocol Buffers deserializers and results.
/// </summary>
/// <remarks>
/// This factory provides a centralized way to create different types of deserializers
/// for Protocol Buffers data, including schema-aware and schema-less deserializers.
/// </remarks>
public class ProtobufFactory : IDeserializerFactory
{
    /// <summary>
    /// Gets the default deserializer options.
    /// </summary>
    public static DeserializerOptions DefaultOptions { get; } = new();
    
    /// <inheritdoc/>
    public IDeserializer<DictionaryProtobufResult> CreateDictionaryDeserializer(ICodedInputStream input, DeserializerOptions? options = null)
    {
        return new DictionaryDeserializer(input, options ?? DefaultOptions);
    }
    
    /// <inheritdoc/>
    public IDeserializer<DynamicProtobufResult> CreateDynamicDeserializer(ICodedInputStream input, DeserializerOptions? options = null)
    {
        return new DynamicDeserializer(input, options ?? DefaultOptions);
    }
    
    /// <inheritdoc/>
    public IDeserializer<DynamicProtobufResult> CreateSchemaDeserializer(
        ICodedInputStream input,
        string protoFileContent,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return new ProtobufDeserializer(input, rootMessageName, protoFileContent, null, options ?? DefaultOptions);
    }
    
    /// <inheritdoc/>
    public IDeserializer<DynamicProtobufResult> CreateDirectorySchemaDeserializer(
        ICodedInputStream input,
        string protoDirectory,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return new ProtobufDeserializer(input, rootMessageName, null, protoDirectory, options ?? DefaultOptions);
    }
    
    /// <summary>
    /// Creates a new dictionary-based deserializer for the specified byte array.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A deserializer for dictionary-based results.</returns>
    public static IDeserializer<DictionaryProtobufResult> CreateDictionaryDeserializer(byte[] data, DeserializerOptions? options = null)
    {
        return new DictionaryDeserializer(new CodedInputStream(data), options ?? DefaultOptions);
    }
    
    /// <summary>
    /// Creates a new dynamic object-based deserializer for the specified byte array.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A deserializer for dynamic object-based results.</returns>
    public static IDeserializer<DynamicProtobufResult> CreateDynamicDeserializer(byte[] data, DeserializerOptions? options = null)
    {
        return new DynamicDeserializer(new CodedInputStream(data), options ?? DefaultOptions);
    }
    
    /// <summary>
    /// Creates a schema-aware deserializer for the specified byte array.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoFileContent">The content of the .proto file defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A schema-aware deserializer for Protocol Buffers messages.</returns>
    public static IDeserializer<DynamicProtobufResult> CreateSchemaDeserializer(
        byte[] data,
        string protoFileContent,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return new ProtobufDeserializer(
            new CodedInputStream(data),
            rootMessageName,
            protoFileContent,
            null,
            options ?? DefaultOptions);
    }
    
    /// <summary>
    /// Creates a schema-aware deserializer for the specified byte array using multiple .proto files from a directory.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoDirectory">The directory containing .proto files defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A schema-aware deserializer for Protocol Buffers messages.</returns>
    public static IDeserializer<DynamicProtobufResult> CreateDirectorySchemaDeserializer(
        byte[] data,
        string protoDirectory,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return new ProtobufDeserializer(
            new CodedInputStream(data),
            rootMessageName,
            null,
            protoDirectory,
            options ?? DefaultOptions);
    }
    
    /// <summary>
    /// Directly deserializes Protocol Buffers data from a byte array to a dictionary-based result.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dictionary-based result containing the deserialized data.</returns>
    public static DictionaryProtobufResult DeserializeToDictionary(byte[] data, DeserializerOptions? options = null)
    {
        var deserializer = CreateDictionaryDeserializer(data, options);
        return deserializer.Deserialize();
    }
    
    /// <summary>
    /// Directly deserializes Protocol Buffers data from a byte array to a dynamic object-based result.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object-based result containing the deserialized data.</returns>
    public static DynamicProtobufResult DeserializeToDynamic(byte[] data, DeserializerOptions? options = null)
    {
        var deserializer = CreateDynamicDeserializer(data, options);
        return deserializer.Deserialize();
    }
    
    /// <summary>
    /// Directly deserializes Protocol Buffers data using a .proto file schema.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoFileContent">The content of the .proto file defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object representing the deserialized message according to the schema.</returns>
    public static DynamicProtobufResult DeserializeWithSchema(
        byte[] data,
        string protoFileContent,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        var deserializer = CreateSchemaDeserializer(data, protoFileContent, rootMessageName, options);
        return deserializer.Deserialize();
    }
    
    /// <summary>
    /// Directly deserializes Protocol Buffers data using multiple .proto files from a directory.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoDirectory">The directory containing .proto files defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object representing the deserialized message according to the schema.</returns>
    public static DynamicProtobufResult DeserializeWithDirectorySchema(
        byte[] data,
        string protoDirectory,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        var deserializer = CreateDirectorySchemaDeserializer(data, protoDirectory, rootMessageName, options);
        return deserializer.Deserialize();
    }
}
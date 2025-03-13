using Protobuf.Core.Interfaces;

namespace Protobuf.NET.Interfaces;

/// <summary>
/// Provides factory methods for creating deserializers.
/// </summary>
public interface IDeserializerFactory
{
    /// <summary>
    /// Creates a deserializer for dictionary-based results.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A deserializer for dictionary-based results.</returns>
    IDeserializer<DictionaryProtobufResult> CreateDictionaryDeserializer(
        ICodedInputStream input, 
        DeserializerOptions? options = null);
    
    /// <summary>
    /// Creates a deserializer for dynamic object-based results.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A deserializer for dynamic object-based results.</returns>
    IDeserializer<DynamicProtobufResult> CreateDynamicDeserializer(
        ICodedInputStream input, 
        DeserializerOptions? options = null);

    /// <summary>
    /// Creates a schema-aware deserializer that uses a .proto file to interpret the message structure.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="protoFileContent">The content of the .proto file defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A schema-aware deserializer for Protocol Buffers messages.</returns>
    public IDeserializer<DynamicProtobufResult> CreateSchemaDeserializer(
        ICodedInputStream input,
        string protoFileContent,
        string rootMessageName,
        DeserializerOptions? options = null);

    /// <summary>
    /// Creates a schema-aware deserializer that uses multiple .proto files from a directory to interpret the message structure.
    /// </summary>
    /// <param name="input">The input stream containing Protocol Buffers data.</param>
    /// <param name="protoDirectory">The directory containing .proto files defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A schema-aware deserializer for Protocol Buffers messages.</returns>
    public IDeserializer<DynamicProtobufResult> CreateDirectorySchemaDeserializer(
        ICodedInputStream input,
        string protoDirectory,
        string rootMessageName,
        DeserializerOptions? options = null);
}
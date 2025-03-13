namespace Protobuf.NET.Extensions;

/// <summary>
/// Provides extension methods for Protocol Buffers deserialization.
/// </summary>
public static class ProtobufExtensions
{
    /// <summary>
    /// Deserializes the byte array to a dictionary-based result.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dictionary-based result containing the deserialized data.</returns>
    public static DictionaryProtobufResult DeserializeAsDictionary(this byte[] data, DeserializerOptions? options = null)
    {
        return ProtobufFactory.DeserializeToDictionary(data, options);
    }
    
    /// <summary>
    /// Deserializes the byte array to a dynamic object-based result.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object-based result containing the deserialized data.</returns>
    public static DynamicProtobufResult DeserializeAsDynamic(this byte[] data, DeserializerOptions? options = null)
    {
        return ProtobufFactory.DeserializeToDynamic(data, options);
    }
    
    /// <summary>
    /// Deserializes the byte array using a .proto file schema to a structured dynamic object.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoFileContent">The content of the .proto file defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object representing the deserialized message according to the schema.</returns>
    public static DynamicProtobufResult DeserializeWithSchema(
        this byte[] data,
        string protoFileContent,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return ProtobufFactory.DeserializeWithSchema(data, protoFileContent, rootMessageName, options);
    }
    
    /// <summary>
    /// Deserializes the byte array using multiple .proto files from a directory to a structured dynamic object.
    /// </summary>
    /// <param name="data">The byte array containing Protocol Buffers data.</param>
    /// <param name="protoDirectory">The directory containing .proto files defining the message structure.</param>
    /// <param name="rootMessageName">The fully qualified name of the root message type (including package).</param>
    /// <param name="options">Options to configure the deserialization process.</param>
    /// <returns>A dynamic object representing the deserialized message according to the schema.</returns>
    public static DynamicProtobufResult DeserializeWithDirectorySchema(
        this byte[] data,
        string protoDirectory,
        string rootMessageName,
        DeserializerOptions? options = null)
    {
        return ProtobufFactory.DeserializeWithDirectorySchema(data, protoDirectory, rootMessageName, options);
    }
}
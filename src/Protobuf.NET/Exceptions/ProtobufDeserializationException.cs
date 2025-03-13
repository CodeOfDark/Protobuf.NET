namespace Protobuf.NET.Exceptions;

/// <summary>
/// Exception thrown when an error occurs during Protocol Buffers deserialization.
/// </summary>
public class ProtobufDeserializationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufDeserializationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ProtobufDeserializationException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufDeserializationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ProtobufDeserializationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
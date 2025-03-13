namespace Protobuf.NET.Exceptions;

/// <summary>
/// Exception thrown when the maximum nesting depth is exceeded during Protocol Buffers deserialization.
/// </summary>
public class ProtobufMaxNestingDepthExceededException : ProtobufDeserializationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtobufMaxNestingDepthExceededException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ProtobufMaxNestingDepthExceededException(string message) : base(message)
    {
    }
}
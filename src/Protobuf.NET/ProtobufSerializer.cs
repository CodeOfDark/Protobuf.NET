using Protobuf.NET.Interfaces;

namespace Protobuf.NET;

/// <summary>
/// Static utility class for Protocol Buffer serialization
/// </summary>
public static class ProtobufSerializer
{
    private static readonly IMessageSerializer Serializer = new MessageSerializer();

    /// <summary>
    /// Serializes a dynamic object to Protocol Buffer binary format
    /// </summary>
    /// <param name="message">The dynamic object to serialize</param>
    /// <returns>Serialized byte array</returns>
    public static byte[] Serialize(dynamic message)
    {
        return Serializer.Serialize(message);
    }
}
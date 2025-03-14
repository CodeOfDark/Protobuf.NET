namespace Protobuf.NET.Interfaces;

/// <summary>
/// Interface for Protocol Buffer message serialization
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Serializes a dynamic object to Protocol Buffer binary format
    /// </summary>
    /// <param name="message">The dynamic object to serialize</param>
    /// <returns>Serialized byte array</returns>
    byte[] Serialize(dynamic message);
}
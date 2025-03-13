using Protobuf.Core.Interfaces;

namespace Protobuf.NET.Interfaces;

/// <summary>
/// Represents a generic deserializer for Protocol Buffers data.
/// </summary>
/// <typeparam name="TResult">The type of result to deserialize to.</typeparam>
/// <remarks>
/// Implementations of this interface provide deserialization logic for specific result types.
/// </remarks>
public interface IDeserializer<out TResult> where TResult : IProtobufResult
{
    /// <summary>
    /// Deserializes Protocol Buffers data from the specified input stream.
    /// </summary>
    /// <param name="input">Optional secondary input stream. If null, the main input stream is used.</param>
    /// <returns>A deserialized result of type <typeparamref name="TResult"/>.</returns>
    TResult Deserialize(ICodedInputStream? input = null);
}
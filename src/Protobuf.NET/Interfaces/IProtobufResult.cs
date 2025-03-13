namespace Protobuf.NET.Interfaces;

/// <summary>
/// Represents the base interface for all Protocol Buffers result types.
/// </summary>
/// <remarks>
/// This interface serves as a marker interface for all deserialized Protocol Buffers results,
/// allowing for type constraints in generic methods without requiring specific implementations.
/// </remarks>
public interface IProtobufResult
{
    /// <summary>
    /// Gets a string representation of this result.
    /// </summary>
    /// <returns>A string representation of the result.</returns>
    string ToString();
}
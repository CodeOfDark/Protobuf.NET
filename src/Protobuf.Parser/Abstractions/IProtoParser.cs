using Protobuf.Parser.Models;

namespace Protobuf.Parser.Abstractions;

/// <summary>
/// Interface for Protocol Buffer parser implementations.
/// </summary>
public interface IProtoParser
{
    /// <summary>
    /// Parses a Protocol Buffer file from the specified path.
    /// </summary>
    /// <param name="filePath">The path to the .proto file.</param>
    /// <returns>A <see cref="ProtoFile"/> containing the parsed definition.</returns>
    ProtoFile ParseFile(string filePath);
        
    /// <summary>
    /// Parses Protocol Buffer content directly from a string.
    /// </summary>
    /// <param name="content">The Protocol Buffer content as a string.</param>
    /// <param name="fileName">Optional filename for reference (used in error messages).</param>
    /// <returns>A <see cref="ProtoFile"/> containing the parsed definition.</returns>
    ProtoFile ParseContent(string content, string fileName = "");
        
    /// <summary>
    /// Parses a directory of .proto files into a collection of files.
    /// </summary>
    /// <param name="directoryPath">The directory containing .proto files.</param>
    /// <returns>A collection of <see cref="ProtoFile"/> objects.</returns>
    ICollection<ProtoFile> ParseDirectory(string directoryPath);
}
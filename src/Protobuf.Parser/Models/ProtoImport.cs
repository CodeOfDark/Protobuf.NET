namespace Protobuf.Parser.Models;

/// <summary>
/// Represents an import statement in a Protocol Buffer file.
/// </summary>
public class ProtoImport
{
    /// <summary>
    /// Gets or sets the import path.
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// Gets or sets whether this is a public import.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets whether this is a weak import.
    /// </summary>
    public bool IsWeak { get; set; }
        
    /// <summary>
    /// Gets or sets the resolved file for this import.
    /// </summary>
    public ProtoFile? ResolvedFile { get; set; }
}
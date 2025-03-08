namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a parsed Protocol Buffer file.
/// </summary>
public class ProtoFile
{
    /// <summary>
    /// Gets or sets the file name of the Protocol Buffer file.
    /// </summary>
    public string FileName { get; set; } = "";

    /// <summary>
    /// Gets or sets the full file path of the Protocol Buffer file.
    /// </summary>
    public string FilePath { get; set; } = "";

    /// <summary>
    /// Gets or sets the raw content of the Protocol Buffer file.
    /// </summary>
    public string RawContent { get; set; } = "";

    /// <summary>
    /// Gets or sets the syntax version (proto2 or proto3).
    /// </summary>
    public string Syntax { get; set; } = "proto2";

    /// <summary>
    /// Gets or sets the package name.
    /// </summary>
    public string Package { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of imports.
    /// </summary>
    public List<ProtoImport> Imports { get; set; } = [];

    /// <summary>
    /// Gets or sets the file-level options.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of messages defined in the file.
    /// </summary>
    public List<ProtoMessage> Messages { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of enums defined in the file.
    /// </summary>
    public List<ProtoEnum> Enums { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of services defined in the file.
    /// </summary>
    public List<ProtoService> Services { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of extensions defined in the file.
    /// </summary>
    public List<ProtoExtension> Extensions { get; set; } = [];

    /// <summary>
    /// Gets or sets the comments in the file, mapped by line number.
    /// </summary>
    public Dictionary<int, string> Comments { get; set; } = new();
}
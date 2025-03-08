namespace Protobuf.Parser.Models;

/// <summary>
/// Represents a method in a Protocol Buffer service.
/// </summary>
public class ProtoMethod
{
    /// <summary>
    /// Gets or sets the method name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the input type.
    /// </summary>
    public string InputType { get; set; } = "";

    /// <summary>
    /// Gets or sets the output type.
    /// </summary>
    public string OutputType { get; set; } = "";

    /// <summary>
    /// Gets or sets whether the client sends a stream of messages.
    /// </summary>
    public bool ClientStreaming { get; set; }

    /// <summary>
    /// Gets or sets whether the server sends a stream of messages.
    /// </summary>
    public bool ServerStreaming { get; set; }

    /// <summary>
    /// Gets or sets the list of method options.
    /// </summary>
    public List<ProtoOption> Options { get; set; } = [];
        
    /// <summary>
    /// Gets or sets the parent service.
    /// </summary>
    public ProtoService? ParentService { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedInputType.
    /// </summary>
    public object? ResolvedInputType { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedInputFile.
    /// </summary>
    public ProtoFile? ResolvedInputFile { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedOutputType.
    /// </summary>
    public object? ResolvedOutputType { get; set; }
    
    /// <summary>
    /// Gets or sets the resolvedOutputFile.
    /// </summary>
    public ProtoFile? ResolvedOutputFile { get; set; }
}
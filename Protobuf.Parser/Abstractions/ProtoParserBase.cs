using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Models;

namespace Protobuf.Parser.Abstractions;

/// <summary>
/// Base class for Protocol Buffer parser implementations.
/// </summary>
public abstract class ProtoParserBase : IProtoParser
{
    /// <summary>
    /// Gets the tokenizer used by this parser.
    /// </summary>
    protected ITokenizer Tokenizer { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParserBase"/> class.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to use.</param>
    protected ProtoParserBase(ITokenizer tokenizer)
    {
        Tokenizer = tokenizer;
    }
    
    /// <inheritdoc/>
    public virtual ProtoFile ParseFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Protocol Buffer file not found: {filePath}");
        
        try
        {
            var content = File.ReadAllText(filePath);
            var file = ParseContent(content, Path.GetFileName(filePath));
            file.FilePath = filePath;
            return file;
        }
        catch (ProtoParseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ProtoParseException($"Failed to parse {filePath}: {ex.Message}", ex);
        }
    }
    
    /// <inheritdoc/>
    public abstract ProtoFile ParseContent(string content, string fileName = "");
    
    /// <inheritdoc/>
    public virtual ICollection<ProtoFile> ParseDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        
        var files = Directory.GetFiles(directoryPath, "*.proto", SearchOption.AllDirectories).Select(ParseFile).ToList();
        ResolveReferences(files);
        
        return files;
    }
    
    /// <summary>
    /// Resolves references between Protocol Buffer files.
    /// </summary>
    /// <param name="files">The files to resolve references for.</param>
    protected virtual void ResolveReferences(ICollection<ProtoFile> files)
    {
    }
}
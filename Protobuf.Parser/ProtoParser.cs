using Protobuf.Parser.Abstractions;
using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;
using Protobuf.Parser.Utilities;

namespace Protobuf.Parser;

/// <summary>
/// Main parser for Protocol Buffer files.
/// </summary>
public class ProtoParser : ProtoParserBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParser"/> class.
    /// </summary>
    public ProtoParser() : base(new ProtoTokenizer())
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParser"/> class with a custom tokenizer.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to use.</param>
    public ProtoParser(ITokenizer tokenizer) : base(tokenizer)
    {
    }
        
    /// <inheritdoc/>
    public override ProtoFile ParseContent(string content, string fileName = "")
    {
        try
        {
            var tokens = Tokenizer.Tokenize(content).ToList();
            var commentMap = CommentExtractor.ExtractComments(tokens);
            
            var filteredTokens = tokens.Where(t => t.Type != TokenType.Comment && t.Type != TokenType.Whitespace).ToList();
            
            var tokenReader = new TokenReader(filteredTokens, fileName);
            var fileParser = new ProtoFileParser(tokenReader);
            
            var protoFile = fileParser.Parse();
            
            protoFile.Comments = commentMap;
            protoFile.RawContent = content;
            
            return protoFile;
        }
        catch (Exception ex) when (ex is not ProtoParseException)
        {
            throw new ProtoParseException($"Failed to parse Protocol Buffer content{(string.IsNullOrEmpty(fileName) ? "" : $" in {fileName}")}: {ex.Message}", ex);
        }
    }
        
    /// <inheritdoc/>
    protected override void ResolveReferences(ICollection<ProtoFile> files)
    {
        var fileMap = files.ToDictionary(f => f.FilePath, f => f);
        var packageMap = new Dictionary<string, List<ProtoFile>>();
        
        foreach (var file in files)
        {
            if (string.IsNullOrEmpty(file.Package)) 
                continue;
            
            if (!packageMap.TryGetValue(file.Package, out var value))
            {
                value = [];
                packageMap[file.Package] = value;
            }

            value.Add(file);
        }
        
        foreach (var file in files)
        {
            foreach (var import in file.Imports)
            {
                var importPath = PathUtils.ResolveImportPath(file.FilePath, import.Path);
                if (fileMap.TryGetValue(importPath, out var importedFile))
                    import.ResolvedFile = importedFile;
            }
        }
            
        // TODO: Resolve other cross-file references (message types, enum types, etc.)
    }
}
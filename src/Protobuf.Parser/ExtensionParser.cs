using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer extension definitions.
/// </summary>
public class ExtensionParser
{
    private readonly TokenReader _tokenReader;
    private readonly FieldParser _fieldParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensionParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public ExtensionParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _fieldParser = new FieldParser(tokenReader);
    }
        
    /// <summary>
    /// Parses an extension definition.
    /// </summary>
    /// <returns>The parsed extension.</returns>
    public ProtoExtension Parse()
    {
        _tokenReader.Advance();
     
        var extendedTypeToken = _tokenReader.Expect(TokenType.Identifier);
        var extendedType = extendedTypeToken.Value;
        
        var extension = new ProtoExtension
        {
            ExtendedType = extendedType
        };
        
        _tokenReader.Expect(TokenType.LBrace);
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (_tokenReader.Check(TokenType.Optional) || _tokenReader.Check(TokenType.Required) || _tokenReader.Check(TokenType.Repeated) || _tokenReader.Check(TokenType.Identifier))
                extension.Fields.Add(_fieldParser.Parse());
            else
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in extension body");
        }
        
        _tokenReader.Expect(TokenType.RBrace);
        return extension;
    }
}
using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer oneof definitions.
/// </summary>
public class OneofParser
{
    private readonly TokenReader _tokenReader;
    private readonly OptionParser _optionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="OneofParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public OneofParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _optionParser = new OptionParser(tokenReader);
    }
        
    /// <summary>
    /// Parses a oneof definition.
    /// </summary>
    /// <returns>The parsed oneof.</returns>
    public ProtoOneof Parse()
    {
        _tokenReader.Advance();
        
        var oneofNameToken = _tokenReader.Expect(TokenType.Identifier);
        var oneofName = oneofNameToken.Value;
        
        var oneof = new ProtoOneof
        {
            Name = oneofName
        };
        
        _tokenReader.Expect(TokenType.LBrace);
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (_tokenReader.Check(TokenType.Option))
            {
                _optionParser.Parse();
            }
            else if (_tokenReader.Check(TokenType.Identifier))
            {
                var field = ParseOneofField();
                field.IsOneofField = true;
                oneof.Fields.Add(field);
            }
            else
            {
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in oneof body");
            }
        }
        
        _tokenReader.Expect(TokenType.RBrace);
        return oneof;
    }
        
    private ProtoField ParseOneofField()
    {
        var typeToken = _tokenReader.Expect(TokenType.Identifier);
        var fieldType = typeToken.Value;
        
        if (_tokenReader.Check(TokenType.LAngle))
        {
            _tokenReader.Advance();
            var genericTypeToken = _tokenReader.Expect(TokenType.Identifier);
            var genericType = genericTypeToken.Value;
            _tokenReader.Expect(TokenType.RAngle);
            fieldType += $"<{genericType}>";
        }
        
        var nameToken = _tokenReader.Expect(TokenType.Identifier);
        var fieldName = nameToken.Value;
        
        _tokenReader.Expect(TokenType.Equals);
        
        var numberToken = _tokenReader.Expect(TokenType.IntegerLiteral);
        if (!int.TryParse(numberToken.Value, out var fieldNumber))
            throw _tokenReader.CreateParseException($"Invalid field number: {numberToken.Value}");
        
        var field = new ProtoField
        {
            Type = fieldType,
            Name = fieldName,
            Number = fieldNumber
        };
        
        if (_tokenReader.Check(TokenType.LBracket))
            field.Options = _optionParser.ParseFieldOptions();
        
        _tokenReader.Expect(TokenType.Semicolon);
        return field;
    }
}
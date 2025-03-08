using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer field definitions.
/// </summary>
public class FieldParser
{
    private readonly TokenReader _tokenReader;
    private readonly OptionParser _optionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public FieldParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _optionParser = new OptionParser(tokenReader);
    }
        
    /// <summary>
    /// Parses a field definition.
    /// </summary>
    /// <returns>The parsed field.</returns>
    public ProtoField Parse()
    {
        var field = new ProtoField();
        
        if (_tokenReader.Check(TokenType.Optional) || _tokenReader.Check(TokenType.Required) || _tokenReader.Check(TokenType.Repeated))
        {
            field.Rule = _tokenReader.Current().Value;
            _tokenReader.Advance();
        }
        
        var typeToken = _tokenReader.Expect(TokenType.Identifier);
        field.Type = typeToken.Value;
        
        if (_tokenReader.Check(TokenType.LAngle))
        {
            _tokenReader.Advance();
            var genericTypeToken = _tokenReader.Expect(TokenType.Identifier);
            field.GenericType = genericTypeToken.Value;
            _tokenReader.Expect(TokenType.RAngle);
        }
        
        var nameToken = _tokenReader.Expect(TokenType.Identifier);
        field.Name = nameToken.Value;
        
        _tokenReader.Expect(TokenType.Equals);
        
        var numberToken = _tokenReader.Expect(TokenType.IntegerLiteral);
        if (!int.TryParse(numberToken.Value, out var fieldNumber))
            throw _tokenReader.CreateParseException($"Invalid field number: {numberToken.Value}");
        
        field.Number = fieldNumber;
        if (_tokenReader.Check(TokenType.LBracket))
            field.Options = _optionParser.ParseFieldOptions();
        
        _tokenReader.Expect(TokenType.Semicolon);
        return field;
    }
        
    /// <summary>
    /// Parses a map field definition.
    /// </summary>
    /// <returns>The parsed map field.</returns>
    public ProtoField ParseMapField()
    {
        _tokenReader.Advance();
        _tokenReader.Expect(TokenType.LAngle);
        
        var keyTypeToken = _tokenReader.Expect(TokenType.Identifier);
        var keyType = keyTypeToken.Value;
        
        _tokenReader.Expect(TokenType.Comma);
        
        var valueTypeToken = _tokenReader.Expect(TokenType.Identifier);
        var valueType = valueTypeToken.Value;
        
        if (_tokenReader.Check(TokenType.LAngle))
        {
            _tokenReader.Advance();
            var genericTypeToken = _tokenReader.Expect(TokenType.Identifier);
            valueType += "<" + genericTypeToken.Value + ">";
            _tokenReader.Expect(TokenType.RAngle);
        }
        
        _tokenReader.Expect(TokenType.RAngle);
        
        var nameToken = _tokenReader.Expect(TokenType.Identifier);
        var fieldName = nameToken.Value;
        
        _tokenReader.Expect(TokenType.Equals);
        
        var numberToken = _tokenReader.Expect(TokenType.IntegerLiteral);
        if (!int.TryParse(numberToken.Value, out var fieldNumber))
            throw _tokenReader.CreateParseException($"Invalid field number: {numberToken.Value}");
        
        var field = new ProtoField
        {
            IsMap = true,
            KeyType = keyType,
            Type = valueType,
            Name = fieldName,
            Number = fieldNumber
        };
        
        if (_tokenReader.Check(TokenType.LBracket))
            field.Options = _optionParser.ParseFieldOptions();
        
        _tokenReader.Expect(TokenType.Semicolon);
        return field;
    }
}
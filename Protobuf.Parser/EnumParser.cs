using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer enum definitions.
/// </summary>
public class EnumParser
{
    private readonly TokenReader _tokenReader;
    private readonly OptionParser _optionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public EnumParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _optionParser = new OptionParser(tokenReader);
    }
        
    /// <summary>
    /// Parses an enum definition.
    /// </summary>
    /// <param name="parentMessage">The parent message name, if any.</param>
    /// <returns>The parsed enum.</returns>
    public ProtoEnum Parse(string? parentMessage = null)
    {
        _tokenReader.Advance();
        
        var enumNameToken = _tokenReader.Expect(TokenType.Identifier);
        var enumName = enumNameToken.Value;
        
        var protoEnum = new ProtoEnum
        {
            Name = enumName,
            ParentMessage = parentMessage
        };
        
        _tokenReader.Expect(TokenType.LBrace);
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (_tokenReader.Check(TokenType.Option))
                protoEnum.Options.Add(_optionParser.Parse());
            else if (_tokenReader.Check(TokenType.Identifier))
                protoEnum.Values.Add(ParseEnumValue());
            else if (_tokenReader.Check(TokenType.Reserved))
                ParseEnumReserved();
            else
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in enum body");
        }
        
        _tokenReader.Expect(TokenType.RBrace);
        return protoEnum;
    }
        
    private ProtoEnumValue ParseEnumValue()
    {
        var nameToken = _tokenReader.Expect(TokenType.Identifier);
        var valueName = nameToken.Value;
        
        _tokenReader.Expect(TokenType.Equals);
        
        var numberToken = _tokenReader.Expect(TokenType.IntegerLiteral);
        if (!int.TryParse(numberToken.Value, out int valueNumber))
            throw _tokenReader.CreateParseException($"Invalid enum value number: {numberToken.Value}");
        
        var enumValue = new ProtoEnumValue
        {
            Name = valueName,
            Number = valueNumber
        };
        
        if (_tokenReader.Check(TokenType.LBracket))
            enumValue.Options = _optionParser.ParseFieldOptions();
        
        _tokenReader.Expect(TokenType.Semicolon);
        return enumValue;
    }
        
    private void ParseEnumReserved()
    {
        _tokenReader.Advance();
        
        if (_tokenReader.Check(TokenType.StringLiteral))
        {
            do
            {
                _tokenReader.Advance();
                
                if (_tokenReader.Check(TokenType.Comma))
                    _tokenReader.Advance();
                else
                    break;
                
            } while (_tokenReader.Check(TokenType.StringLiteral));
        }
        else while (_tokenReader.Check(TokenType.IntegerLiteral))
        {
            _tokenReader.Advance();
            
            if (_tokenReader.Check(TokenType.To))
            {
                _tokenReader.Advance();
                _tokenReader.Expect(TokenType.IntegerLiteral);
            }
                    
            if (_tokenReader.Check(TokenType.Comma))
                _tokenReader.Advance();
            else
                break;
        }
        
        _tokenReader.Expect(TokenType.Semicolon);
    }
}
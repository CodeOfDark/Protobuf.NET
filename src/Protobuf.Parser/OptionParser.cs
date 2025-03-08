using System.Text;
using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer option definitions.
/// </summary>
public class OptionParser
{
    private readonly TokenReader _tokenReader;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public OptionParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
    }
        
    /// <summary>
    /// Parses an option definition.
    /// </summary>
    /// <returns>The parsed option.</returns>
    public ProtoOption Parse()
    {
        _tokenReader.Advance();
        var optionName = ParseOptionName();
        
        _tokenReader.Expect(TokenType.Equals);
        var optionValue = ParseOptionValue();
        
        _tokenReader.Expect(TokenType.Semicolon);
        return new ProtoOption
        {
            Name = optionName,
            Value = optionValue
        };
    }
        
    /// <summary>
    /// Parses field options.
    /// </summary>
    /// <returns>The parsed field options.</returns>
    public List<ProtoOption> ParseFieldOptions()
    {
        var options = new List<ProtoOption>();
        
        _tokenReader.Expect(TokenType.LBracket);
        do
        {
            var optionName = ParseOptionName();
            
            _tokenReader.Expect(TokenType.Equals);
            var optionValue = ParseOptionValue();
            
            options.Add(new ProtoOption
            {
                Name = optionName,
                Value = optionValue
            });
            
            if (_tokenReader.Check(TokenType.Comma))
                _tokenReader.Advance();
            else
                break;
            
        } while (!_tokenReader.Check(TokenType.RBracket) && !_tokenReader.IsAtEnd());
        
        _tokenReader.Expect(TokenType.RBracket);
        return options;
    }
        
    private string ParseOptionName()
    {
        if (!_tokenReader.Check(TokenType.LParen)) 
            return ParseDottedIdentifier();
        
        _tokenReader.Advance();
        var name = ParseDottedIdentifier();
        _tokenReader.Expect(TokenType.RParen);
        return "(" + name + ")";

    }
        
    private string ParseDottedIdentifier()
    {
        var identifier = _tokenReader.Expect(TokenType.Identifier);
        var name = identifier.Value;
            
        while (_tokenReader.Check(TokenType.Dot))
        {
            _tokenReader.Advance();
            var nextPart = _tokenReader.Expect(TokenType.Identifier);
            name += "." + nextPart.Value;
        }
        
        return name;
    }
        
    private string ParseOptionValue()
    {
        if (_tokenReader.Check(TokenType.StringLiteral) || _tokenReader.Check(TokenType.Identifier) || 
            _tokenReader.Check(TokenType.IntegerLiteral) || _tokenReader.Check(TokenType.FloatLiteral) || 
            _tokenReader.Check(TokenType.BoolLiteral))
            return _tokenReader.Advance().Value;

        if (_tokenReader.Check(TokenType.LBrace))
            return ParseComplexOptionValue();

        throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} for option value");
    }
        
    private string ParseComplexOptionValue()
    {
        var builder = new StringBuilder();
        builder.Append('{');
        _tokenReader.Advance();
            
        var first = true;
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (!first)
            {
                if (_tokenReader.Check(TokenType.Comma))
                {
                    builder.Append(',');
                    _tokenReader.Advance();
                }
                else
                {
                    break;
                }
            }
            
            var nameToken = _tokenReader.Expect(TokenType.Identifier);
            builder.Append(' ').Append(nameToken.Value);
            
            _tokenReader.Expect(TokenType.Colon);
            builder.Append(": ");
            
            if (_tokenReader.Check(TokenType.StringLiteral) || _tokenReader.Check(TokenType.Identifier) || 
                _tokenReader.Check(TokenType.IntegerLiteral) || _tokenReader.Check(TokenType.FloatLiteral) || 
                _tokenReader.Check(TokenType.BoolLiteral))
                builder.Append(_tokenReader.Advance().Value);
            else if (_tokenReader.Check(TokenType.LBrace))
                builder.Append(ParseComplexOptionValue());
            else
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} for complex option value");
                
            first = false;
        }
            
        builder.Append(" }");
        _tokenReader.Expect(TokenType.RBrace);
            
        return builder.ToString();
    }
}
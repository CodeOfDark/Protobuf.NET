using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer reserved field definitions.
/// </summary>
public class ReservedParser
{
    private readonly TokenReader _tokenReader;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ReservedParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public ReservedParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
    }
        
    /// <summary>
    /// Parses a reserved definition.
    /// </summary>
    /// <returns>The parsed reserved definition.</returns>
    public ProtoReserved Parse()
    {
        _tokenReader.Advance();
        
        var reserved = new ProtoReserved();
        if (_tokenReader.Check(TokenType.StringLiteral))
        {
            reserved.IsName = true;
            
            do
            {
                var nameToken = _tokenReader.Expect(TokenType.StringLiteral);
                var fieldName = nameToken.Value.Trim('"', '\'');
                reserved.FieldNames.Add(fieldName);
                    
                if (_tokenReader.Check(TokenType.Comma))
                    _tokenReader.Advance();
                else
                    break;
                
            } while (_tokenReader.Check(TokenType.StringLiteral) && !_tokenReader.IsAtEnd());
        }
        else if (_tokenReader.Check(TokenType.IntegerLiteral))
        {
            reserved.IsName = false;
            
            do
            {
                var startToken = _tokenReader.Expect(TokenType.IntegerLiteral);
                var start = int.Parse(startToken.Value);
                var end = start;
                    
                if (_tokenReader.Check(TokenType.To))
                {
                    _tokenReader.Advance();
                    var endToken = _tokenReader.Expect(TokenType.IntegerLiteral);
                    end = int.Parse(endToken.Value);
                }
                    
                reserved.Ranges.Add(new ProtoRange { From = start, To = end });
                if (_tokenReader.Check(TokenType.Comma))
                    _tokenReader.Advance();
                else
                    break;
                
            } while (_tokenReader.Check(TokenType.IntegerLiteral) && !_tokenReader.IsAtEnd());
        }
        else
        {
            throw _tokenReader.CreateParseException($"Expected string literal or integer for reserved, got {_tokenReader.Current()}");
        }
        
        _tokenReader.Expect(TokenType.Semicolon);
        return reserved;
    }
}
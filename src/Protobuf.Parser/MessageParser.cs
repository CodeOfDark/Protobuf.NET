using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer message definitions.
/// </summary>
public class MessageParser
{
    private readonly TokenReader _tokenReader;
    private readonly EnumParser _enumParser;
    private readonly FieldParser _fieldParser;
    private readonly OneofParser _oneofParser;
    private readonly OptionParser _optionParser;
    private readonly ReservedParser _reservedParser;
    private readonly ExtensionParser _extensionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public MessageParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _fieldParser = new FieldParser(tokenReader);
        _oneofParser = new OneofParser(tokenReader);
        _optionParser = new OptionParser(tokenReader);
        _reservedParser = new ReservedParser(tokenReader);
        _extensionParser = new ExtensionParser(tokenReader);
        _enumParser = new EnumParser(tokenReader);
    }
        
    /// <summary>
    /// Parses a message definition.
    /// </summary>
    /// <param name="parentMessage">The parent message name.</param>
    /// <returns>The parsed message.</returns>
    public ProtoMessage Parse(string? parentMessage = null)
    {
        _tokenReader.Advance();
        
        var messageNameToken = _tokenReader.Expect(TokenType.Identifier);
        var messageName = messageNameToken.Value;
        
        var message = new ProtoMessage
        {
            Name = messageName,
            ParentMessage = parentMessage
        };
        
        _tokenReader.Expect(TokenType.LBrace);
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (_tokenReader.Check(TokenType.Option))
                message.Options.Add(_optionParser.Parse());
            else if (_tokenReader.Check(TokenType.Message))
                message.NestedMessages.Add(Parse(message.FullName));
            else if (_tokenReader.Check(TokenType.Enum))
                message.NestedEnums.Add(_enumParser.Parse(message.FullName));
            else if (_tokenReader.Check(TokenType.Oneof))
                message.Oneofs.Add(_oneofParser.Parse());
            else if (_tokenReader.Check(TokenType.Map))
                message.Fields.Add(_fieldParser.ParseMapField());
            else if (_tokenReader.Check(TokenType.Reserved))
                message.Reserved.Add(_reservedParser.Parse());
            else if (_tokenReader.Check(TokenType.Extensions))
                ParseExtensionsRange();
            else if (_tokenReader.Check(TokenType.Extend))
                message.Extensions.Add(_extensionParser.Parse());
            else if (_tokenReader.Check(TokenType.Optional) || _tokenReader.Check(TokenType.Required) || _tokenReader.Check(TokenType.Repeated) || _tokenReader.Check(TokenType.Identifier))
                message.Fields.Add(_fieldParser.Parse());
            else
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in message body");
        }
        
        _tokenReader.Expect(TokenType.RBrace);
        return message;
    }
        
    private void ParseExtensionsRange()
    {
        _tokenReader.Advance();
        ParseRange();
        _tokenReader.Expect(TokenType.Semicolon);
    }

    private void ParseRange()
    {
        while (true)
        {
            _tokenReader.Expect(TokenType.IntegerLiteral);
            if (_tokenReader.Check(TokenType.To))
            {
                _tokenReader.Advance();

                if (_tokenReader.Check(TokenType.IntegerLiteral))
                    _tokenReader.Advance();
                else if (_tokenReader.Current().Value == "max")
                    _tokenReader.Advance();
                else
                    throw _tokenReader.CreateParseException($"Expected integer or 'max' for range end, got {_tokenReader.Current()}");
            }

            if (!_tokenReader.Check(TokenType.Comma)) 
                return;

            _tokenReader.Advance();
        }
    }
}
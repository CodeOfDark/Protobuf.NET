using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Parser for Protocol Buffer files.
/// </summary>
public class ProtoFileParser
{
    private readonly TokenReader _tokenReader;
    private readonly MessageParser _messageParser;
    private readonly EnumParser _enumParser;
    private readonly ServiceParser _serviceParser;
    private readonly OptionParser _optionParser;
    private readonly ExtensionParser _extensionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoFileParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public ProtoFileParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        
        _messageParser = new MessageParser(tokenReader);
        _enumParser = new EnumParser(tokenReader);
        _serviceParser = new ServiceParser(tokenReader);
        _optionParser = new OptionParser(tokenReader);
        _extensionParser = new ExtensionParser(tokenReader);
    }
        
    /// <summary>
    /// Parses the tokens into a ProtoFile object.
    /// </summary>
    /// <returns>The parsed Protocol Buffer file.</returns>
    public ProtoFile Parse()
    {
        var protoFile = new ProtoFile();
        
        while (!_tokenReader.IsAtEnd())
        {
            switch (_tokenReader.Current().Type)
            {
                case TokenType.Syntax:
                    protoFile.Syntax = ParseSyntax();
                    break;
                    
                case TokenType.Package:
                    protoFile.Package = ParsePackage();
                    break;
                    
                case TokenType.Import:
                    protoFile.Imports.Add(ParseImport());
                    break;
                    
                case TokenType.Option:
                    protoFile.Options.Add(_optionParser.Parse());
                    break;
                    
                case TokenType.Message:
                    protoFile.Messages.Add(_messageParser.Parse());
                    break;
                    
                case TokenType.Enum:
                    protoFile.Enums.Add(_enumParser.Parse());
                    break;
                    
                case TokenType.Service:
                    protoFile.Services.Add(_serviceParser.Parse());
                    break;
                    
                case TokenType.Extend:
                    protoFile.Extensions.Add(_extensionParser.Parse());
                    break;
                    
                case TokenType.EndOfFile:
                    return protoFile;
                        
                default:
                    throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} at top level");
            }
        }
            
        return protoFile;
    }
        
    private string ParseSyntax()
    {
        _tokenReader.Advance();
        _tokenReader.Expect(TokenType.Equals);
        
        var syntaxToken = _tokenReader.Expect(TokenType.StringLiteral);
        var syntaxValue = syntaxToken.Value.Trim('"', '\'');
        
        _tokenReader.Expect(TokenType.Semicolon);
        return syntaxValue;
    }
        
    private string ParsePackage()
    {
        _tokenReader.Advance();

        var firstPart = _tokenReader.Expect(TokenType.Identifier);
        var packageName = firstPart.Value;
            
        while (_tokenReader.Check(TokenType.Dot))
        {
            _tokenReader.Advance();
            var nextPart = _tokenReader.Expect(TokenType.Identifier);
            packageName += "." + nextPart.Value;
        }
        
        _tokenReader.Expect(TokenType.Semicolon);
        return packageName;
    }
        
    private ProtoImport ParseImport()
    {
        _tokenReader.Advance();
        
        var isPublic = false;
        var isWeak = false;
            
        if (_tokenReader.Check(TokenType.Public))
        {
            isPublic = true;
            _tokenReader.Advance();
        }
        else if (_tokenReader.Check(TokenType.Weak))
        {
            isWeak = true;
            _tokenReader.Advance();
        }
        
        var importPathToken = _tokenReader.Expect(TokenType.StringLiteral);
        var importPath = importPathToken.Value.Trim('"', '\'');
        
        _tokenReader.Expect(TokenType.Semicolon);
        return new ProtoImport
        {
            Path = importPath,
            IsPublic = isPublic,
            IsWeak = isWeak
        };
    }
}
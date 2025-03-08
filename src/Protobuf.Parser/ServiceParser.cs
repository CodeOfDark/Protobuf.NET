using Protobuf.Parser.Lexing;
using Protobuf.Parser.Models;

namespace Protobuf.Parser;

/// <summary>
/// Specialized parser for Protocol Buffer service definitions.
/// </summary>
public class ServiceParser
{
    private readonly TokenReader _tokenReader;
    private readonly OptionParser _optionParser;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceParser"/> class.
    /// </summary>
    /// <param name="tokenReader">The token reader to use.</param>
    public ServiceParser(TokenReader tokenReader)
    {
        _tokenReader = tokenReader;
        _optionParser = new OptionParser(tokenReader);
    }
        
    /// <summary>
    /// Parses a service definition.
    /// </summary>
    /// <returns>The parsed service.</returns>
    public ProtoService Parse()
    {
        _tokenReader.Advance();
        
        var serviceNameToken = _tokenReader.Expect(TokenType.Identifier);
        var serviceName = serviceNameToken.Value;
        
        var service = new ProtoService
        {
            Name = serviceName
        };
        
        _tokenReader.Expect(TokenType.LBrace);
        while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
        {
            if (_tokenReader.Check(TokenType.Option))
                service.Options.Add(_optionParser.Parse());
            else if (_tokenReader.Check(TokenType.Rpc))
                service.Methods.Add(ParseMethod());
            else
                throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in service body");
        }
        
        _tokenReader.Expect(TokenType.RBrace);
        return service;
    }
        
    private ProtoMethod ParseMethod()
    {
        _tokenReader.Advance();
        var methodNameToken = _tokenReader.Expect(TokenType.Identifier);
        var methodName = methodNameToken.Value;
        
        _tokenReader.Expect(TokenType.LParen);
        
        var clientStreaming = false;
        if (_tokenReader.Check(TokenType.Stream))
        {
            clientStreaming = true;
            _tokenReader.Advance();
        }
        
        var inputTypeToken = _tokenReader.Expect(TokenType.Identifier);
        var inputType = inputTypeToken.Value;
        
        _tokenReader.Expect(TokenType.RParen);
        _tokenReader.Expect(TokenType.Returns);
        _tokenReader.Expect(TokenType.LParen);
        
        var serverStreaming = false;
        if (_tokenReader.Check(TokenType.Stream))
        {
            serverStreaming = true;
            _tokenReader.Advance();
        }
        
        var outputTypeToken = _tokenReader.Expect(TokenType.Identifier);
        var outputType = outputTypeToken.Value;
        
        _tokenReader.Expect(TokenType.RParen);
        var method = new ProtoMethod
        {
            Name = methodName,
            InputType = inputType,
            OutputType = outputType,
            ClientStreaming = clientStreaming,
            ServerStreaming = serverStreaming
        };
        
        if (_tokenReader.Check(TokenType.LBrace))
        {
            _tokenReader.Advance();
            while (!_tokenReader.Check(TokenType.RBrace) && !_tokenReader.IsAtEnd())
            {
                if (_tokenReader.Check(TokenType.Option))
                    method.Options.Add(_optionParser.Parse());
                else
                    throw _tokenReader.CreateParseException($"Unexpected token {_tokenReader.Current()} in method options");
            }
                
            _tokenReader.Expect(TokenType.RBrace);
        }
        else
        {
            _tokenReader.Expect(TokenType.Semicolon);
        }

        return method;
    }
}
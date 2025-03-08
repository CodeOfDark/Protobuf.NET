using Protobuf.Parser.Exceptions;
using Protobuf.Parser.Lexing;

namespace Protobuf.Parser;

/// <summary>
/// Helper class for reading tokens from a token stream.
/// </summary>
public class TokenReader
{
    private readonly List<Token> _tokens;
    private readonly string _fileName;
    private int _position;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenReader"/> class.
    /// </summary>
    /// <param name="tokens">The tokens to read.</param>
    /// <param name="fileName">The name of the file being parsed.</param>
    public TokenReader(List<Token> tokens, string fileName)
    {
        _tokens = tokens;
        _fileName = fileName;
        _position = 0;
    }
        
    /// <summary>
    /// Gets the current token.
    /// </summary>
    /// <returns>The current token.</returns>
    public Token Current()
    {
        return _position < _tokens.Count ? _tokens[_position] : _tokens.Last();
    }
        
    /// <summary>
    /// Advances to the next token and returns the previous current token.
    /// </summary>
    /// <returns>The token that was current before advancing.</returns>
    public Token Advance()
    {
        var current = Current();
        _position++;
        return current;
    }
        
    /// <summary>
    /// Expects a token of the specified type.
    /// </summary>
    /// <param name="type">The expected token type.</param>
    /// <returns>The expected token.</returns>
    /// <exception cref="ProtoParseException">Thrown if the current token is not of the expected type.</exception>
    public Token Expect(TokenType type)
    {
        var current = Current();
        if (current.Type != type)
            throw CreateParseException($"Expected token type {type}, got {current.Type}");
        return Advance();
    }
        
    /// <summary>
    /// Checks if the current token is of the specified type.
    /// </summary>
    /// <param name="type">The token type to check for.</param>
    /// <returns><c>true</c> if the current token is of the specified type; otherwise, <c>false</c>.</returns>
    public bool Check(TokenType type)
    {
        return !IsAtEnd() && Current().Type == type;
    }
        
    /// <summary>
    /// Gets whether the end of the token stream has been reached.
    /// </summary>
    /// <returns><c>true</c> if the end of the token stream has been reached; otherwise, <c>false</c>.</returns>
    public bool IsAtEnd()
    {
        return _position >= _tokens.Count || Current().Type == TokenType.EndOfFile;
    }
        
    /// <summary>
    /// Creates a new parse exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new parse exception.</returns>
    public ProtoParseException CreateParseException(string message)
    {
        var token = Current();
        return new ProtoParseException(message, _fileName, token.Line, token.Column);
    }
}
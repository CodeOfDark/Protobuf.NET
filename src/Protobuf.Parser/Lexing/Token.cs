namespace Protobuf.Parser.Lexing;

/// <summary>
/// Represents a token from the Protocol Buffer lexer.
/// </summary>
public class Token
{
    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    public TokenType Type { get; set; }
        
    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public string Value { get; set; }
        
    /// <summary>
    /// Gets or sets the line number where the token appears.
    /// </summary>
    public int Line { get; set; }
        
    /// <summary>
    /// Gets or sets the column number where the token appears.
    /// </summary>
    public int Column { get; set; }
        
    /// <summary>
    /// Creates a new token.
    /// </summary>
    public Token(TokenType type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }
        
    /// <summary>
    /// Returns a string representation of the token for debugging.
    /// </summary>
    public override string ToString() => $"[{Type}] '{Value}' at {Line}:{Column}";
}
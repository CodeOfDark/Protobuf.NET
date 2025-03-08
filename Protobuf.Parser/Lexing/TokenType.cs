namespace Protobuf.Parser.Lexing;

/// <summary>
/// Token types for the Protocol Buffer lexer.
/// </summary>
public enum TokenType
{
    Syntax,
    Package,
    Import,
    Public,
    Weak,
    Option,
    Message,
    Enum,
    Service,
    Rpc,
    Returns,
    Oneof,
    Map,
    Extend,
    Extensions,
    Reserved,
    To,
    Stream,
    Group,
    
    Optional,
    Required,
    Repeated,
    
    Identifier,
    StringLiteral,
    IntegerLiteral,
    FloatLiteral,
    BoolLiteral,
    
    Semicolon,
    Colon,
    Comma,
    Dot,
    Equals,
    LParen,
    RParen,
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    LAngle,
    RAngle,
    Slash,
    
    Comment,
    Whitespace,
    EndOfFile,
    Unknown
}
using System.Text;
using Protobuf.Parser.Abstractions;
using Protobuf.Parser.Exceptions;

namespace Protobuf.Parser.Lexing;

/// <summary>
/// Tokenizer for Protocol Buffer files.
/// </summary>
public class ProtoTokenizer : ITokenizer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "syntax", TokenType.Syntax },
        { "package", TokenType.Package },
        { "import", TokenType.Import },
        { "public", TokenType.Public },
        { "weak", TokenType.Weak },
        { "option", TokenType.Option },
        { "message", TokenType.Message },
        { "enum", TokenType.Enum },
        { "service", TokenType.Service },
        { "rpc", TokenType.Rpc },
        { "returns", TokenType.Returns },
        { "oneof", TokenType.Oneof },
        { "map", TokenType.Map },
        { "extend", TokenType.Extend },
        { "extensions", TokenType.Extensions },
        { "reserved", TokenType.Reserved },
        { "to", TokenType.To },
        { "stream", TokenType.Stream },
        { "group", TokenType.Group },
        { "optional", TokenType.Optional },
        { "required", TokenType.Required },
        { "repeated", TokenType.Repeated },
        { "true", TokenType.BoolLiteral },
        { "false", TokenType.BoolLiteral }
    };
        
    /// <summary>
    /// Tokenizes the input string into a sequence of tokens.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>A sequence of tokens.</returns>
    public IEnumerable<Token> Tokenize(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            yield return new Token(TokenType.EndOfFile, "", 1, 1);
            yield break;
        }
            
        var position = 0;
        var line = 1;
        var column = 1;
            
        while (position < input.Length)
        {
            var current = input[position];
            if (char.IsWhiteSpace(current))
            {
                var (newPos, newLine, newCol) = SkipWhitespace(input, position, line, column);
                position = newPos;
                line = newLine;
                column = newCol;
                continue;
            }
            
            if (current == '/' && position + 1 < input.Length)
            {
                var next = input[position + 1];
                if (next is '/' or '*')
                {
                    var (commentToken, newPos, newLine, newCol) = TokenizeComment(input, position, line, column);
                    yield return commentToken;
                    position = newPos;
                    line = newLine;
                    column = newCol;
                    continue;
                }
            }
            
            if (current is '"' or '\'')
            {
                var (stringToken, newPos, newLine, newCol) = TokenizeStringLiteral(input, position, line, column);
                yield return stringToken;
                position = newPos;
                line = newLine;
                column = newCol;
                continue;
            }
            
            if (IsSymbol(current))
            {
                var (symbolToken, newPos, newCol) = TokenizeSymbol(input, position, line, column);
                yield return symbolToken;
                position = newPos;
                column = newCol;
                continue;
            }
            
            if (char.IsLetterOrDigit(current) || current == '_' || current == '.')
            {
                var (token, newPos, newCol) = TokenizeIdentifierOrNumber(input, position, line, column);
                yield return token;
                position = newPos;
                column = newCol;
                continue;
            }
            
            yield return new Token(TokenType.Unknown, current.ToString(), line, column);
            position++;
            column++;
        }
        
        yield return new Token(TokenType.EndOfFile, "", line, column);
    }
        
    private static (int, int, int) SkipWhitespace(string input, int position, int line, int column)
    {
        var pos = position;
        var lin = line;
        var col = column;
            
        while (pos < input.Length && char.IsWhiteSpace(input[pos]))
        {
            if (input[pos] == '\n')
            {
                lin++;
                col = 1;
            }
            else
            {
                col++;
            }
                
            pos++;
        }
            
        return (pos, lin, col);
    }
        
    private static (Token, int, int, int) TokenizeComment(string input, int position, int line, int column)
    {
        var startLine = line;
        var startColumn = column;
        var builder = new StringBuilder();
        
        builder.Append(input[position]);
        position++;
        column++;
            
        if (input[position] == '/')
        {
            builder.Append(input[position]);
            position++;
            column++;
            
            while (position < input.Length && input[position] != '\n')
            {
                builder.Append(input[position]);
                position++;
                column++;
            }
        }
        else if (input[position] == '*')
        {
            builder.Append(input[position]);
            position++;
            column++;
                
            var foundEnd = false;
            while (position < input.Length && !foundEnd)
            {
                var c = input[position];
                builder.Append(c);
                
                if (c == '*' && position + 1 < input.Length && input[position + 1] == '/')
                {
                    builder.Append(input[position + 1]);
                    position += 2;
                    column += 2;
                    foundEnd = true;
                    break;
                }
                
                if (c == '\n')
                {
                    line++;
                    column = 1;
                }
                else
                {
                    column++;
                }
                    
                position++;
            }
                
            if (!foundEnd)
                throw new ProtoTokenizerException("Unterminated block comment", startLine, startColumn);
        }
            
        return (new Token(TokenType.Comment, builder.ToString(), startLine, startColumn), position, line, column);
    }
        
    private static (Token, int, int, int) TokenizeStringLiteral(string input, int position, int line, int column)
    {
        var startColumn = column;
        var builder = new StringBuilder();
            
        var quoteChar = input[position];
        builder.Append(quoteChar);
        position++;
        column++;
            
        var foundEnd = false;
        while (position < input.Length && !foundEnd)
        {
            var c = input[position];
            builder.Append(c);
                
            if (c == quoteChar && (position == 0 || input[position - 1] != '\\'))
                foundEnd = true;
            else if (c == '\n')
                throw new ProtoTokenizerException("Unterminated string literal", line, startColumn);
                
            position++;
            column++;
        }
            
        if (!foundEnd)
            throw new ProtoTokenizerException("Unterminated string literal", line, startColumn);
            
        return (new Token(TokenType.StringLiteral, builder.ToString(), line, startColumn), position, line, column);
    }
        
    private static (Token, int, int) TokenizeSymbol(string input, int position, int line, int column)
    {
        var c = input[position];

        var type = c switch
        {
            ';' => TokenType.Semicolon,
            ':' => TokenType.Colon,
            ',' => TokenType.Comma,
            '.' => TokenType.Dot,
            '=' => TokenType.Equals,
            '(' => TokenType.LParen,
            ')' => TokenType.RParen,
            '{' => TokenType.LBrace,
            '}' => TokenType.RBrace,
            '[' => TokenType.LBracket,
            ']' => TokenType.RBracket,
            '<' => TokenType.LAngle,
            '>' => TokenType.RAngle,
            '/' => TokenType.Slash,
            _ => TokenType.Unknown
        };

        return (new Token(type, c.ToString(), line, column), position + 1, column + 1);
    }
        
    private static (Token, int, int) TokenizeIdentifierOrNumber(string input, int position, int line, int column)
    {
        var startColumn = column;
        var builder = new StringBuilder();
        
        var firstChar = input[position];
        builder.Append(firstChar);
        position++;
        column++;
        
        while (position < input.Length)
        {
            var c = input[position];
            if (char.IsLetterOrDigit(c) || c == '_' || c == '.')
            {
                builder.Append(c);
                position++;
                column++;
            }
            else
            {
                break;
            }
        }
            
        var value = builder.ToString();
        if (Keywords.TryGetValue(value, out TokenType keywordType))
            return (new Token(keywordType, value, line, startColumn), position, column);
            
        if (char.IsDigit(firstChar) || (firstChar == '-' && position < input.Length && char.IsDigit(input[position])))
        {
            if (value.Contains('.'))
                return (new Token(TokenType.FloatLiteral, value, line, startColumn), position, column);
            
            return (new Token(TokenType.IntegerLiteral, value, line, startColumn), position, column);
        }
        
        return (new Token(TokenType.Identifier, value, line, startColumn), position, column);
    }
        
    private static bool IsSymbol(char c) 
        => c is ';' or ':' or ',' or '.' or '=' or '(' or ')' or '{' or '}' or '[' or ']' or '<' or '>' or '/';
}
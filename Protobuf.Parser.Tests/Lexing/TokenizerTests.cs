using Protobuf.Parser.Lexing;

namespace Protobuf.Parser.Tests.Lexing;

public class TokenizerTests
{
    private readonly ProtoTokenizer _tokenizer = new();

    [Fact]
    public void EmptyString()
    {
        const string input = "";
        var tokens = _tokenizer.Tokenize(input).ToList();
     
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfFile, tokens[0].Type);
    }
        
    [Fact]
    public void Whitespace()
    {
        const string input = "  \t\r\n  ";
        var tokens = _tokenizer.Tokenize(input).ToList();
            
        Assert.Single(tokens);
        Assert.Equal(TokenType.EndOfFile, tokens[0].Type);
    }
        
    [Fact]
    public void Keywords()
    {
        const string input = "syntax package import option message enum service rpc returns oneof map extend extensions reserved to";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
            
        Assert.Equal(15, tokens.Count);
        Assert.Equal(TokenType.Syntax, tokens[0].Type);
        Assert.Equal(TokenType.Package, tokens[1].Type);
        Assert.Equal(TokenType.Import, tokens[2].Type);
        Assert.Equal(TokenType.Option, tokens[3].Type);
        Assert.Equal(TokenType.Message, tokens[4].Type);
        Assert.Equal(TokenType.Enum, tokens[5].Type);
        Assert.Equal(TokenType.Service, tokens[6].Type);
        Assert.Equal(TokenType.Rpc, tokens[7].Type);
        Assert.Equal(TokenType.Returns, tokens[8].Type);
        Assert.Equal(TokenType.Oneof, tokens[9].Type);
        Assert.Equal(TokenType.Map, tokens[10].Type);
        Assert.Equal(TokenType.Extend, tokens[11].Type);
        Assert.Equal(TokenType.Extensions, tokens[12].Type);
        Assert.Equal(TokenType.Reserved, tokens[13].Type);
        Assert.Equal(TokenType.To, tokens[14].Type);
    }
        
    [Fact]
    public void FieldRuleKeywords()
    {
        const string input = "optional required repeated";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(3, tokens.Count);
        Assert.Equal(TokenType.Optional, tokens[0].Type);
        Assert.Equal(TokenType.Required, tokens[1].Type);
        Assert.Equal(TokenType.Repeated, tokens[2].Type);
    }
        
    [Fact]
    public void BoolLiterals()
    {
        const string input = "true false";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.BoolLiteral, tokens[0].Type);
        Assert.Equal("true", tokens[0].Value);
        Assert.Equal(TokenType.BoolLiteral, tokens[1].Type);
        Assert.Equal("false", tokens[1].Value);
    }
        
    [Fact]
    public void Identifiers()
    {
        const string input = "myMessage MyEnum _privateField field1 camelCase snake_case";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(6, tokens.Count);
        foreach (var token in tokens)
            Assert.Equal(TokenType.Identifier, token.Type);
        
        Assert.Equal("myMessage", tokens[0].Value);
        Assert.Equal("MyEnum", tokens[1].Value);
        Assert.Equal("_privateField", tokens[2].Value);
        Assert.Equal("field1", tokens[3].Value);
        Assert.Equal("camelCase", tokens[4].Value);
        Assert.Equal("snake_case", tokens[5].Value);
    }
        
    [Fact]
    public void Numbers()
    {
        const string input = "123 456.789";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.Equal("123", tokens[0].Value);
        Assert.Equal(TokenType.FloatLiteral, tokens[1].Type);
        Assert.Equal("456.789", tokens[1].Value);
    }
        
    [Fact]
    public void StringLiterals()
    {
        const string input = "\"double quoted string\" 'single quoted string'";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.StringLiteral, tokens[0].Type);
        Assert.Equal("\"double quoted string\"", tokens[0].Value);
        Assert.Equal(TokenType.StringLiteral, tokens[1].Type);
        Assert.Equal("'single quoted string'", tokens[1].Value);
    }
        
    [Fact]
    public void Symbols()
    {
        const string input = "; : , . = ( ) { } [ ] < > /";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(14, tokens.Count);
        Assert.Equal(TokenType.Semicolon, tokens[0].Type);
        Assert.Equal(TokenType.Colon, tokens[1].Type);
        Assert.Equal(TokenType.Comma, tokens[2].Type);
        Assert.Equal(TokenType.Dot, tokens[3].Type);
        Assert.Equal(TokenType.Equals, tokens[4].Type);
        Assert.Equal(TokenType.LParen, tokens[5].Type);
        Assert.Equal(TokenType.RParen, tokens[6].Type);
        Assert.Equal(TokenType.LBrace, tokens[7].Type);
        Assert.Equal(TokenType.RBrace, tokens[8].Type);
        Assert.Equal(TokenType.LBracket, tokens[9].Type);
        Assert.Equal(TokenType.RBracket, tokens[10].Type);
        Assert.Equal(TokenType.LAngle, tokens[11].Type);
        Assert.Equal(TokenType.RAngle, tokens[12].Type);
        Assert.Equal(TokenType.Slash, tokens[13].Type);
    }
        
    [Fact]
    public void LineComment()
    {
        const string input = "// This is a line comment";
        var tokens = _tokenizer.Tokenize(input).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Comment, tokens[0].Type);
        Assert.Equal("// This is a line comment", tokens[0].Value);
    }
        
    [Fact]
    public void BlockComment()
    {
        const string input = "/* This is a\nblock comment */";
        var tokens = _tokenizer.Tokenize(input).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Comment, tokens[0].Type);
        Assert.Equal("/* This is a\nblock comment */", tokens[0].Value);
    }
        
    [Fact]
    public void AsteriskComment()
    {
        const string input = "/**\n * This is a JavaDoc-style comment\n * with asterisks.\n */";
        var tokens = _tokenizer.Tokenize(input).ToList();
        
        Assert.Equal(2, tokens.Count);
        Assert.Equal(TokenType.Comment, tokens[0].Type);
    }
        
    [Fact]
    public void CompleteProtoSyntax()
    {
        const string input = "syntax = \"proto3\";\npackage example;\nmessage Person {\nstring name = 1;\nint32 id = 2;\nrepeated string emails = 3;\n}";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
        
        Assert.Equal(27, tokens.Count);
        Assert.Equal(TokenType.Syntax, tokens[0].Type);
        Assert.Equal(TokenType.Equals, tokens[1].Type);
        Assert.Equal(TokenType.StringLiteral, tokens[2].Type);
        Assert.Equal(TokenType.Semicolon, tokens[3].Type);
        Assert.Equal(TokenType.Package, tokens[4].Type);
        Assert.Equal(TokenType.Identifier, tokens[5].Type);
        Assert.Equal(TokenType.Semicolon, tokens[6].Type);
        Assert.Equal(TokenType.Message, tokens[7].Type);
        Assert.Equal(TokenType.Identifier, tokens[8].Type);
        Assert.Equal(TokenType.LBrace, tokens[9].Type);
        Assert.Equal(TokenType.Identifier, tokens[10].Type);
        Assert.Equal(TokenType.Identifier, tokens[11].Type);
        Assert.Equal(TokenType.Equals, tokens[12].Type);
        Assert.Equal(TokenType.IntegerLiteral, tokens[13].Type);
        Assert.Equal(TokenType.Semicolon, tokens[14].Type);
    }
        
    [Fact]
    public void TracksLineAndColumnCorrectly()
    {
        const string input = "syntax = \"proto3\";\npackage example;";
        var tokens = _tokenizer.Tokenize(input).Where(t => t.Type != TokenType.EndOfFile).ToList();
     
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);
        Assert.Equal(1, tokens[1].Line);
        Assert.Equal(8, tokens[1].Column);
        Assert.Equal(2, tokens[4].Line);
        Assert.Equal(1, tokens[4].Column);
    }
}
using Protobuf.Parser.Lexing;

namespace Protobuf.Parser.Abstractions;

/// <summary>
/// Interface for Protocol Buffer tokenizer implementations.
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// Tokenizes the input string into a sequence of tokens.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    /// <returns>A sequence of tokens.</returns>
    IEnumerable<Token> Tokenize(string input);
}
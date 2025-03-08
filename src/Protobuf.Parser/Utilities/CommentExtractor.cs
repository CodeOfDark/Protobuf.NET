using System.Text;
using Protobuf.Parser.Lexing;

namespace Protobuf.Parser.Utilities;

/// <summary>
/// Utility class for extracting and processing comments.
/// </summary>
public static class CommentExtractor
{
    /// <summary>
    /// Extracts comments from the token stream.
    /// </summary>
    /// <param name="tokens">The token stream to extract comments from.</param>
    /// <returns>A dictionary mapping line numbers to comment text.</returns>
    public static Dictionary<int, string> ExtractComments(IEnumerable<Token> tokens)
    {
        var commentMap = new Dictionary<int, string>();
            
        foreach (var token in tokens.Where(t => t.Type == TokenType.Comment))
        {
            var lineNumber = token.Line;
            var commentText = token.Value;
            
            commentText = ProcessComment(commentText);
            commentMap[lineNumber] = commentText;
        }
            
        return commentMap;
    }
        
    /// <summary>
    /// Processes a comment text.
    /// </summary>
    /// <param name="comment">The comment text to process.</param>
    /// <returns>The processed comment text.</returns>
    public static string ProcessComment(string comment)
    {
        if (comment.StartsWith("//"))
            return comment[2..].Trim();
        
        if (comment.StartsWith("/*") && comment.EndsWith("*/"))
        {
            if (comment.StartsWith("/**"))
                return ProcessJavaDocComment(comment);
                
            return comment.Substring(2, comment.Length - 4).Trim();
        }
        
        return comment;
    }
        
    /// <summary>
    /// Processes a JavaDoc-style comment.
    /// </summary>
    /// <param name="comment">The JavaDoc-style comment to process.</param>
    /// <returns>The processed comment text.</returns>
    private static string ProcessJavaDocComment(string comment)
    {
        var content = comment.Substring(3, comment.Length - 5);
        var lines = content.Split('\n');
        var processedLines = new StringBuilder();
            
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith('*'))
                trimmed = trimmed[1..].TrimStart();
                
            if (processedLines.Length > 0)
                processedLines.AppendLine();
                
            processedLines.Append(trimmed);
        }
            
        return processedLines.ToString().Trim();
    }
}
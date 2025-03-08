namespace Protobuf.Parser.Exceptions;

/// <summary>
/// Exception thrown when errors occur during Protocol Buffer parsing.
/// </summary>
public class ProtoParseException : Exception
{
    /// <summary>
    /// Gets or sets the line number where the error occurred.
    /// </summary>
    public int Line { get; set; }
        
    /// <summary>
    /// Gets or sets the column number where the error occurred.
    /// </summary>
    public int Column { get; set; }
        
    /// <summary>
    /// Gets or sets the file name where the error occurred.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParseException"/> class.
    /// </summary>
    public ProtoParseException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProtoParseException(string message) : base(message) { }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParseException"/> class with position information.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="fileName">The file where the error occurred.</param>
    /// <param name="line">The line number where the error occurred.</param>
    /// <param name="column">The column number where the error occurred.</param>
    public ProtoParseException(string message, string fileName, int line, int column) 
        : base($"{message} at {fileName}:{line}:{column}")
    {
        FileName = fileName;
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoParseException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProtoParseException(string message, Exception innerException) : base(message, innerException) { }
}
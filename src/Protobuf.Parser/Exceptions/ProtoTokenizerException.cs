namespace Protobuf.Parser.Exceptions;

/// <summary>
/// Exception thrown when errors occur during Protocol Buffer tokenization.
/// </summary>
public class ProtoTokenizerException : Exception
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
    /// Initializes a new instance of the <see cref="ProtoTokenizerException"/> class.
    /// </summary>
    public ProtoTokenizerException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoTokenizerException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProtoTokenizerException(string message) : base(message) { }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoTokenizerException"/> class with position information.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="line">The line number where the error occurred.</param>
    /// <param name="column">The column number where the error occurred.</param>
    public ProtoTokenizerException(string message, int line, int column) 
        : base($"{message} at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoTokenizerException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProtoTokenizerException(string message, Exception innerException) : base(message, innerException) { }
}
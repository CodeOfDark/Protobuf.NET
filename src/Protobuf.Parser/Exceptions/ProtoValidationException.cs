namespace Protobuf.Parser.Exceptions;

/// <summary>
/// Exception thrown when validation of a Protocol Buffer definition fails.
/// </summary>
public class ProtoValidationException : Exception
{
    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public List<string> ValidationErrors { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoValidationException"/> class.
    /// </summary>
    public ProtoValidationException() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ProtoValidationException(string message) : base(message) 
    {
        ValidationErrors.Add(message);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoValidationException"/> class with a list of validation errors.
    /// </summary>
    /// <param name="validationErrors">The list of validation errors.</param>
    public ProtoValidationException(List<string> validationErrors) 
        : base($"Validation failed with {validationErrors.Count} errors")
    {
        ValidationErrors.AddRange(validationErrors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoValidationException"/> class with a specified error message 
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ProtoValidationException(string message, Exception innerException) : base(message, innerException)
    {
        ValidationErrors.Add(message);
    }
}
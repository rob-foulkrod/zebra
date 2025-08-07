namespace Zebra.Exceptions;

/// <summary>
/// Exception thrown when file operations fail.
/// </summary>
public class FileOperationException : ZebraException
{
    /// <summary>
    /// Gets the file path that caused the exception.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationException"/> class.
    /// </summary>
    public FileOperationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FileOperationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationException"/> class with a specified error message and file path.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="filePath">The file path that caused the exception.</param>
    public FileOperationException(string message, string filePath) : base(message)
    {
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public FileOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOperationException"/> class with a specified error message, file path, and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="filePath">The file path that caused the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public FileOperationException(string message, string filePath, Exception innerException) : base(message, innerException)
    {
        FilePath = filePath;
    }
}
namespace Zebra.Exceptions;

/// <summary>
/// Exception thrown when QR code generation fails.
/// </summary>
public class QrCodeGenerationException : ZebraException
{
    /// <summary>
    /// Gets the URL that caused the exception.
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QrCodeGenerationException"/> class.
    /// </summary>
    public QrCodeGenerationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QrCodeGenerationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public QrCodeGenerationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QrCodeGenerationException"/> class with a specified error message and URL.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="url">The URL that caused the exception.</param>
    public QrCodeGenerationException(string message, string url) : base(message)
    {
        Url = url;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QrCodeGenerationException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public QrCodeGenerationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QrCodeGenerationException"/> class with a specified error message, URL, and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="url">The URL that caused the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public QrCodeGenerationException(string message, string url, Exception innerException) : base(message, innerException)
    {
        Url = url;
    }
}
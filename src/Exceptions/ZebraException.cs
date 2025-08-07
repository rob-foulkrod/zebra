namespace Zebra.Exceptions;

/// <summary>
/// Base exception class for all Zebra-specific exceptions.
/// </summary>
public class ZebraException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZebraException"/> class.
    /// </summary>
    public ZebraException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZebraException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ZebraException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZebraException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ZebraException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
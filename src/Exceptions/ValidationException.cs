using System.ComponentModel.DataAnnotations;

namespace Zebra.Exceptions;

/// <summary>
/// Exception thrown when input validation fails.
/// </summary>
public class ValidationException : ZebraException
{
    /// <summary>
    /// Gets the parameter name that failed validation.
    /// </summary>
    public string? ParameterName { get; }

    /// <summary>
    /// Gets the parameter value that failed validation.
    /// </summary>
    public object? ParameterValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    public ValidationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and parameter name.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="parameterName">The parameter name that failed validation.</param>
    public ValidationException(string message, string parameterName) : base(message)
    {
        ParameterName = parameterName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message, parameter name, and parameter value.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="parameterName">The parameter name that failed validation.</param>
    /// <param name="parameterValue">The parameter value that failed validation.</param>
    public ValidationException(string message, string parameterName, object? parameterValue) : base(message)
    {
        ParameterName = parameterName;
        ParameterValue = parameterValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and a reference to the inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
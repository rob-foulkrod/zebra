using QRCoder;
using System.Drawing;
using Zebra.Exceptions;

namespace Zebra.Services;

/// <summary>
/// Service for generating QR codes and validating URLs.
/// </summary>
public class QrCodeService
{
    private const int MaxUrlLength = 2048; // Standard URL length limit
    private const int MinUrlLength = 3; // Minimum URL length (e.g., "a.b")

    /// <summary>
    /// Generates a Base64-encoded QR code for the specified URL.
    /// </summary>
    /// <param name="url">The URL to encode in the QR code.</param>
    /// <returns>The Base64-encoded QR code image.</returns>
    /// <exception cref="ValidationException">Thrown when URL validation fails.</exception>
    /// <exception cref="QrCodeGenerationException">Thrown when QR code generation fails.</exception>
    public string GenerateQrCodeBase64(string url)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ValidationException("URL cannot be null or empty.", nameof(url));
        }

        if (url.Length < MinUrlLength)
        {
            throw new ValidationException($"URL is too short (minimum {MinUrlLength} characters): {url}", nameof(url), url);
        }

        if (url.Length > MaxUrlLength)
        {
            throw new ValidationException($"URL exceeds maximum length ({MaxUrlLength} characters): {url[..50]}...", nameof(url), url);
        }

        // Check for invalid characters that could cause QR generation issues
        if (ContainsInvalidCharacters(url))
        {
            throw new ValidationException($"URL contains invalid characters: {url}", nameof(url), url);
        }

        try
        {
            var qrCodeBytes = GenerateQrCodeBytes(url);
            return Convert.ToBase64String(qrCodeBytes);
        }
        catch (QrCodeGenerationException)
        {
            throw; // Re-throw QR generation exceptions as-is
        }
        catch (Exception ex)
        {
            throw new QrCodeGenerationException($"Failed to generate QR code for URL '{url}': {ex.Message}", url, ex);
        }
    }

    /// <summary>
    /// Validates if a URL is properly formatted and supported.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid; otherwise, false.</returns>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        if (url.Length < MinUrlLength || url.Length > MaxUrlLength)
        {
            return false;
        }

        if (ContainsInvalidCharacters(url))
        {
            return false;
        }

        try
        {
            var isValidUri = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);
            return isValidUri && 
                   uriResult is not null && 
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a URL and returns detailed validation results.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>A tuple containing validation result and error message if invalid.</returns>
    public static (bool IsValid, string? ErrorMessage) ValidateUrlDetailed(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return (false, "URL cannot be null or empty.");
        }

        if (url.Length < MinUrlLength)
        {
            return (false, $"URL is too short (minimum {MinUrlLength} characters).");
        }

        if (url.Length > MaxUrlLength)
        {
            return (false, $"URL exceeds maximum length ({MaxUrlLength} characters).");
        }

        if (ContainsInvalidCharacters(url))
        {
            return (false, "URL contains invalid characters.");
        }

        try
        {
            var isValidUri = Uri.TryCreate(url, UriKind.Absolute, out var uriResult);
            if (!isValidUri || uriResult is null)
            {
                return (false, "URL format is invalid.");
            }

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                return (false, $"URL scheme '{uriResult.Scheme}' is not supported. Only HTTP and HTTPS are allowed.");
            }

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"URL validation failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates QR code bytes for the specified URL.
    /// </summary>
    /// <param name="url">The URL to encode.</param>
    /// <returns>The QR code as a byte array.</returns>
    /// <exception cref="QrCodeGenerationException">Thrown when QR code generation fails.</exception>
    private static byte[] GenerateQrCodeBytes(string url)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
        catch (ArgumentException ex)
        {
            throw new QrCodeGenerationException($"Invalid argument for QR code generation: {ex.Message}", url, ex);
        }
        catch (OutOfMemoryException ex)
        {
            throw new QrCodeGenerationException($"Insufficient memory to generate QR code: {ex.Message}", url, ex);
        }
        catch (Exception ex)
        {
            throw new QrCodeGenerationException($"QR code generation failed: {ex.Message}", url, ex);
        }
    }

    /// <summary>
    /// Checks if a URL contains characters that could cause issues with QR code generation.
    /// </summary>
    /// <param name="url">The URL to check.</param>
    /// <returns>True if the URL contains invalid characters; otherwise, false.</returns>
    private static bool ContainsInvalidCharacters(string url)
    {
        // Check for null bytes or other control characters that could cause issues
        return url.Any(c => char.IsControl(c) && c != '\t' && c != '\n' && c != '\r');
    }
}

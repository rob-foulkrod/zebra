using System;
using Xunit;
using Zebra.Exceptions;
using Zebra.Services;

namespace Zebra.Services.Tests
{
    public class QrCodeServiceErrorHandlingTests
    {
        private readonly QrCodeService _qrCodeService;

        public QrCodeServiceErrorHandlingTests()
        {
            _qrCodeService = new QrCodeService();
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlIsNull()
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64(null!));
            Assert.Contains("URL cannot be null or empty", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlIsEmpty()
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64(string.Empty));
            Assert.Contains("URL cannot be null or empty", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlIsWhitespace()
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64("   "));
            Assert.Contains("URL cannot be null or empty", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlIsTooShort()
        {
            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64("ab"));
            Assert.Contains("URL is too short (minimum 3 characters)", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlIsTooLong()
        {
            // Arrange
            var longUrl = new string('a', 2049); // Exceeds 2048 character limit

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64(longUrl));
            Assert.Contains("URL exceeds maximum length", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldThrowValidationException_WhenUrlContainsInvalidCharacters()
        {
            // Arrange
            var urlWithInvalidChars = "https://example.com\0test"; // Contains null byte

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _qrCodeService.GenerateQrCodeBase64(urlWithInvalidChars));
            Assert.Contains("URL contains invalid characters", exception.Message);
            Assert.Equal("url", exception.ParameterName);
        }

        [Fact]
        public void GenerateQrCodeBase64_ShouldSucceedWithValidUrl()
        {
            // Arrange
            var validUrl = "https://example.com";

            // Act
            var result = _qrCodeService.GenerateQrCodeBase64(validUrl);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.True(IsValidBase64(result));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsValidUrl_ShouldReturnFalse_WhenUrlIsNullOrEmpty(string? url)
        {
            // Act
            var result = QrCodeService.IsValidUrl(url!);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ab")] // Too short
        [InlineData("a")] // Too short
        public void IsValidUrl_ShouldReturnFalse_WhenUrlIsTooShort(string url)
        {
            // Act
            var result = QrCodeService.IsValidUrl(url);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUrl_ShouldReturnFalse_WhenUrlIsTooLong()
        {
            // Arrange
            var longUrl = new string('a', 2049); // Exceeds 2048 character limit

            // Act
            var result = QrCodeService.IsValidUrl(longUrl);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidUrl_ShouldReturnFalse_WhenUrlContainsInvalidCharacters()
        {
            // Arrange
            var urlWithInvalidChars = "https://example.com\0test"; // Contains null byte

            // Act
            var result = QrCodeService.IsValidUrl(urlWithInvalidChars);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("ftp://example.com")] // Wrong scheme
        [InlineData("file://example.txt")] // Wrong scheme
        [InlineData("invalid-url")] // Invalid format
        [InlineData("not a url at all")] // Invalid format
        public void IsValidUrl_ShouldReturnFalse_WhenUrlHasInvalidSchemeOrFormat(string url)
        {
            // Act
            var result = QrCodeService.IsValidUrl(url);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("https://example.com")]
        [InlineData("http://example.com")]
        [InlineData("https://subdomain.example.com/path?query=value")]
        [InlineData("http://192.168.1.1:8080")]
        public void IsValidUrl_ShouldReturnTrue_WhenUrlIsValid(string url)
        {
            // Act
            var result = QrCodeService.IsValidUrl(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForNullUrl()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed(null!);

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL cannot be null or empty.", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForEmptyUrl()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed(string.Empty);

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL cannot be null or empty.", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForTooShortUrl()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed("ab");

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL is too short (minimum 3 characters).", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForTooLongUrl()
        {
            // Arrange
            var longUrl = new string('a', 2049);

            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed(longUrl);

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL exceeds maximum length (2048 characters).", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForInvalidCharacters()
        {
            // Arrange
            var urlWithInvalidChars = "https://example.com\0test";

            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed(urlWithInvalidChars);

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL contains invalid characters.", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForInvalidFormat()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed("not a url");

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL format is invalid.", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnErrorForUnsupportedScheme()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed("ftp://example.com");

            // Assert
            Assert.False(isValid);
            Assert.Equal("URL scheme 'ftp' is not supported. Only HTTP and HTTPS are allowed.", errorMessage);
        }

        [Fact]
        public void ValidateUrlDetailed_ShouldReturnValidForValidUrl()
        {
            // Act
            var (isValid, errorMessage) = QrCodeService.ValidateUrlDetailed("https://example.com");

            // Assert
            Assert.True(isValid);
            Assert.Null(errorMessage);
        }

        /// <summary>
        /// Helper method to check if a string is valid Base64.
        /// </summary>
        /// <param name="base64String">The string to validate.</param>
        /// <returns>True if valid Base64; otherwise, false.</returns>
        private static bool IsValidBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
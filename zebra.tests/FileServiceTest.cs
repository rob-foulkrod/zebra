using System.IO;
using System.Threading.Tasks;
using Xunit;
using Zebra.Exceptions;

namespace Zebra.Services.Tests
{
    public class FileServiceTest
    {
        private readonly FileService _fileService;

        public FileServiceTest()
        {
            _fileService = new FileService();
        }

        [Fact]
        public async Task ReadUrlsFromFileAsync_ShouldReturnUrls_WhenFileExists()
        {
            // Arrange
            var filePath = "test_urls.txt";
            var fileContent = "https://example.com\nhttps://test.com\n";
            await File.WriteAllTextAsync(filePath, fileContent);

            // Act
            var result = await _fileService.ReadUrlsFromFileAsync(filePath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("https://example.com", result);
            Assert.Contains("https://test.com", result);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task ReadUrlsFromFileAsync_ShouldThrowFileOperationException_WhenFileDoesNotExist()
        {
            // Arrange
            var filePath = "nonexistent_file.txt";

            // Act & Assert
            await Assert.ThrowsAsync<FileOperationException>(() => _fileService.ReadUrlsFromFileAsync(filePath));
        }

        [Fact]
        public async Task WriteHtmlToFileAsync_ShouldWriteContentToFile()
        {
            // Arrange
            var outputPath = "output.html";
            var htmlContent = "<html><body>Test</body></html>";

            // Act
            await _fileService.WriteHtmlToFileAsync(htmlContent, outputPath);

            // Assert
            Assert.True(File.Exists(outputPath));
            var writtenContent = await File.ReadAllTextAsync(outputPath);
            Assert.Equal(htmlContent, writtenContent);

            // Cleanup
            File.Delete(outputPath);
        }

        [Fact]
        public void ValidateInputFile_ShouldReturnTrue_WhenFileExists()
        {
            // Arrange
            var filePath = "existing_file.txt";
            File.WriteAllText(filePath, "Test content");

            // Act
            var result = _fileService.ValidateInputFile(filePath);

            // Assert
            Assert.True(result);

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public void ValidateInputFile_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            // Arrange
            var filePath = "nonexistent_file.txt";

            // Act
            var result = _fileService.ValidateInputFile(filePath);

            // Assert
            Assert.False(result);
        }
    }
}
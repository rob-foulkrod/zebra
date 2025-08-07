using Zebra.Exceptions;

namespace Zebra.Services;

/// <summary>
/// Service for handling file operations.
/// </summary>
public class FileService
{
    private const int MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
    private const int MaxUrlsCount = 10000; // Maximum number of URLs to process

    /// <summary>
    /// Reads URLs from a file asynchronously with comprehensive error handling.
    /// </summary>
    /// <param name="filePath">The path to the input file.</param>
    /// <returns>A list of URLs read from the file.</returns>
    /// <exception cref="ValidationException">Thrown when input validation fails.</exception>
    /// <exception cref="FileOperationException">Thrown when file operations fail.</exception>
    public async Task<List<string>> ReadUrlsFromFileAsync(string filePath)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ValidationException("File path cannot be null or empty.", nameof(filePath));
        }

        try
        {
            // First check if the path exists at all
            if (!File.Exists(filePath) && !Directory.Exists(filePath))
            {
                throw new FileOperationException($"Input file not found: {filePath}", filePath);
            }

            // Validate it's actually a file, not a directory
            if (Directory.Exists(filePath))
            {
                throw new FileOperationException($"Path is a directory, not a file: {filePath}", filePath);
            }

            // Now we know it's a file that exists

            // Check file size to prevent memory issues
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                throw new FileOperationException(
                    $"File size ({fileInfo.Length:N0} bytes) exceeds maximum allowed size ({MaxFileSizeBytes:N0} bytes): {filePath}",
                    filePath);
            }

            // Check file permissions
            if (!IsFileReadable(filePath))
            {
                throw new FileOperationException($"File is not readable or access is denied: {filePath}", filePath);
            }

            var urls = new List<string>();
            string[] lines;

            try
            {
                lines = await File.ReadAllLinesAsync(filePath).ConfigureAwait(false);
            }
            catch (IOException ex)
            {
                throw new FileOperationException($"Failed to read file: {ex.Message}", filePath, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileOperationException($"Access denied when reading file: {ex.Message}", filePath, ex);
            }
            catch (OutOfMemoryException ex)
            {
                throw new FileOperationException($"File is too large to process: {filePath}", filePath, ex);
            }

            // Process lines with validation
            for (int i = 0; i < lines.Length; i++)
            {
                var trimmedLine = lines[i].Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    // Basic length validation for URLs
                    if (trimmedLine.Length > 2048) // Standard URL length limit
                    {
                        throw new ValidationException(
                            $"URL on line {i + 1} exceeds maximum length (2048 characters): {trimmedLine[..50]}...",
                            nameof(filePath));
                    }

                    urls.Add(trimmedLine);

                    // Prevent processing too many URLs
                    if (urls.Count > MaxUrlsCount)
                    {
                        throw new ValidationException(
                            $"File contains too many URLs. Maximum allowed: {MaxUrlsCount}",
                            nameof(filePath));
                    }
                }
            }

            if (urls.Count == 0)
            {
                throw new ValidationException($"File contains no valid URLs: {filePath}", nameof(filePath));
            }

            return urls;
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (FileOperationException)
        {
            throw; // Re-throw file operation exceptions as-is
        }
        catch (Exception ex)
        {
            throw new FileOperationException($"Unexpected error reading file: {ex.Message}", filePath, ex);
        }
    }

    /// <summary>
    /// Writes HTML content to a file asynchronously with comprehensive error handling.
    /// </summary>
    /// <param name="htmlContent">The HTML content to write.</param>
    /// <param name="outputPath">The output file path.</param>
    /// <exception cref="ValidationException">Thrown when input validation fails.</exception>
    /// <exception cref="FileOperationException">Thrown when file operations fail.</exception>
    public async Task WriteHtmlToFileAsync(string htmlContent, string outputPath)
    {
        // Input validation
        if (htmlContent is null)
        {
            throw new ValidationException("HTML content cannot be null.", nameof(htmlContent));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ValidationException("Output path cannot be null or empty.", nameof(outputPath));
        }

        try
        {
            // Validate and create output directory if needed
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                catch (Exception ex)
                {
                    throw new FileOperationException(
                        $"Failed to create output directory '{directory}': {ex.Message}",
                        outputPath,
                        ex);
                }
            }

            // Check if target is a directory
            if (Directory.Exists(outputPath))
            {
                throw new FileOperationException($"Output path is a directory, not a file: {outputPath}", outputPath);
            }

            // Check write permissions for the directory
            var targetDirectory = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
            if (!IsDirectoryWritable(targetDirectory))
            {
                throw new FileOperationException(
                    $"Directory is not writable or access is denied: {targetDirectory}",
                    outputPath);
            }

            try
            {
                await File.WriteAllTextAsync(outputPath, htmlContent).ConfigureAwait(false);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new FileOperationException($"Directory not found: {ex.Message}", outputPath, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new FileOperationException($"Access denied when writing file: {ex.Message}", outputPath, ex);
            }
            catch (IOException ex)
            {
                throw new FileOperationException($"Failed to write file: {ex.Message}", outputPath, ex);
            }
        }
        catch (ValidationException)
        {
            throw; // Re-throw validation exceptions as-is
        }
        catch (FileOperationException)
        {
            throw; // Re-throw file operation exceptions as-is
        }
        catch (Exception ex)
        {
            throw new FileOperationException($"Unexpected error writing file: {ex.Message}", outputPath, ex);
        }
    }

    /// <summary>
    /// Validates if an input file is suitable for processing.
    /// </summary>
    /// <param name="filePath">The path to the input file.</param>
    /// <returns>True if the file is valid for processing; otherwise, false.</returns>
    public bool ValidateInputFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return false;
        }

        try
        {
            // Check if file exists
            if (!File.Exists(filePath))
            {
                return false;
            }

            // Check if it's actually a file, not a directory
            var fileAttributes = File.GetAttributes(filePath);
            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                return false;
            }

            // Check file size
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                return false;
            }

            // Check if file is readable
            return IsFileReadable(filePath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a file is readable.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>True if the file is readable; otherwise, false.</returns>
    private static bool IsFileReadable(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a directory is writable.
    /// </summary>
    /// <param name="directoryPath">The path to the directory.</param>
    /// <returns>True if the directory is writable; otherwise, false.</returns>
    private static bool IsDirectoryWritable(string directoryPath)
    {
        try
        {
            var testFile = Path.Combine(directoryPath, Path.GetRandomFileName());
            using (var stream = File.Create(testFile))
            {
                // File created successfully
            }
            File.Delete(testFile);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

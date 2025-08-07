namespace Zebra.Services;

public class FileService
{
    public async Task<List<string>> ReadUrlsFromFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Input file not found: {filePath}");
        }

        var urls = new List<string>();
        var lines = await File.ReadAllLinesAsync(filePath);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (!string.IsNullOrEmpty(trimmedLine))
            {
                urls.Add(trimmedLine);
            }
        }

        return urls;
    }

    public async Task WriteHtmlToFileAsync(string htmlContent, string outputPath)
    {
        await File.WriteAllTextAsync(outputPath, htmlContent);
    }

    public bool ValidateInputFile(string filePath)
    {
        return File.Exists(filePath);
    }
}

using System.Diagnostics;
using Zebra.Configuration;
using Zebra.Models;

namespace Zebra.Services;

public class ZebraService
{
    private readonly FileService _fileService;
    private readonly QrCodeService _qrCodeService;
    private readonly HtmlService _htmlService;
    private readonly ZebraConfig _config;

    public ZebraService(FileService fileService, QrCodeService qrCodeService, HtmlService htmlService, ZebraConfig config)
    {
        _fileService = fileService;
        _qrCodeService = qrCodeService;
        _htmlService = htmlService;
        _config = config;
    }

    public async Task<ProcessResult> ProcessUrlFileAsync(string inputFilePath, string outputFilePath)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new ProcessResult();

        try
        {
            LogProgress("Reading URLs from file...");
            var urls = await _fileService.ReadUrlsFromFileAsync(inputFilePath);
            result.TotalUrls = urls.Count;

            LogProgress($"Found {urls.Count} URLs to process...");
            var urlQrPairs = ProcessUrls(urls);

            result.SuccessfulUrls = urlQrPairs.Count(p => p.IsValidUrl);
            result.FailedUrls = urlQrPairs.Count(p => !p.IsValidUrl);
            result.Errors = urlQrPairs.Where(p => !p.IsValidUrl).Select(p => p.ErrorMessage ?? "Unknown error").ToList();

            LogProgress("Generating HTML...");
            var htmlContent = _htmlService.GenerateHtml(urlQrPairs, _config.HtmlTitle);

            LogProgress("Writing HTML file...");
            await _fileService.WriteHtmlToFileAsync(htmlContent, outputFilePath);

            result.Success = true;
            LogProgress($"Processing complete! Generated: {outputFilePath}");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            LogProgress($"Error: {ex.Message}");
        }

        stopwatch.Stop();
        result.ProcessingTime = stopwatch.Elapsed;
        return result;
    }

    private List<UrlQrPair> ProcessUrls(List<string> urls)
    {
        var pairs = new List<UrlQrPair>();

        foreach (var url in urls)
        {
            var pair = new UrlQrPair { Url = url };

            if (QrCodeService.IsValidUrl(url))
            {
                LogProgress($"Processing: {url}");
                pair.Base64QrCode = _qrCodeService.GenerateQrCodeBase64(url);
                pair.IsValidUrl = !string.IsNullOrEmpty(pair.Base64QrCode);
                
                if (!pair.IsValidUrl)
                {
                    pair.ErrorMessage = "Failed to generate QR code";
                }
            }
            else
            {
                pair.IsValidUrl = false;
                pair.ErrorMessage = "Invalid URL format";
                LogProgress($"Invalid URL: {url}");
            }

            pairs.Add(pair);
        }

        return pairs;
    }

    private void LogProgress(string message)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}

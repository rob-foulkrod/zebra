using Zebra.Models;
using Zebra.Services;

namespace Zebra.Test;

public class RazorDemo
{
    public static async Task RunDemoAsync()
    {
        Console.WriteLine("🎯 Razor Templates Demo");
        Console.WriteLine("======================");
        
        // Create sample data
        var qrService = new QrCodeService();
        var sampleUrls = new List<UrlQrPair>
        {
            new() { Url = "https://www.github.com", IsValidUrl = true },
            new() { Url = "https://www.stackoverflow.com", IsValidUrl = true },
            new() { Url = "invalid-url", IsValidUrl = false, ErrorMessage = "Invalid URL format" }
        };

        // Generate QR codes for valid URLs
        foreach (var url in sampleUrls.Where(u => u.IsValidUrl))
        {
            url.Base64QrCode = qrService.GenerateQrCodeBase64(url.Url);
        }

        // Generate HTML using both services
        var originalService = new HtmlService();
        var razorService = new RazorHtmlService();

        Console.WriteLine("📄 Generating HTML with Original String Interpolation...");
        var originalHtml = originalService.GenerateHtml(sampleUrls, "Original Template Demo");
        await File.WriteAllTextAsync("demo-original.html", originalHtml);
        Console.WriteLine("✅ Generated: demo-original.html");

        Console.WriteLine("📄 Generating HTML with Razor Templates...");
        var razorHtml = await razorService.GenerateHtmlAsync(sampleUrls, "Razor Template Demo");
        await File.WriteAllTextAsync("demo-razor.html", razorHtml);
        Console.WriteLine("✅ Generated: demo-razor.html");

        Console.WriteLine("\n📊 Comparison:");
        Console.WriteLine($"   Original HTML: {originalHtml.Length:N0} characters");
        Console.WriteLine($"   Razor HTML:    {razorHtml.Length:N0} characters");
        
        Console.WriteLine("\n🔍 Both files generated successfully!");
        Console.WriteLine("   You can open them in a browser to see the results.");
        Console.WriteLine("   The output should be nearly identical in appearance.");
    }
}

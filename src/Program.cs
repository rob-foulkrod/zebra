using Zebra.Configuration;
using Zebra.Models;
using Zebra.Services;
using Zebra.Test;

namespace Zebra;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🦓 Zebra QR Code Generator");
        Console.WriteLine("==========================");

        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        // Check for test command
        if (args[0].Equals("--test", StringComparison.OrdinalIgnoreCase) || 
            args[0].Equals("-t", StringComparison.OrdinalIgnoreCase))
        {
            await RazorHtmlServiceTest.RunTestsAsync();
            return;
        }

        // Check for demo command
        if (args[0].Equals("--demo", StringComparison.OrdinalIgnoreCase) || 
            args[0].Equals("-d", StringComparison.OrdinalIgnoreCase))
        {
            await RazorDemo.RunDemoAsync();
            return;
        }

        var inputFile = args[0];
        var outputFile = args.Length > 1 ? args[1] : GetOutputFileName(inputFile);

        // Validate input file
        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"❌ Error: Input file '{inputFile}' not found.");
            return;
        }

        // Initialize services
        var config = new ZebraConfig();
        var fileService = new FileService();
        var qrCodeService = new QrCodeService();
        var htmlService = new HtmlService();
        var zebraService = new ZebraService(fileService, qrCodeService, htmlService, config);

        // Process the file
        var result = await zebraService.ProcessUrlFileAsync(inputFile, outputFile);

        // Display results
        DisplayResults(result);
    }

    private static void ShowUsage()
    {
        Console.WriteLine("Usage: zebra <input-file> [output-file]");
        Console.WriteLine("       zebra --test | -t");
        Console.WriteLine("       zebra --demo | -d");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  input-file   Text file containing URLs (one per line)");
        Console.WriteLine("  output-file  HTML output file (optional, defaults to qr-codes.html)");
        Console.WriteLine("  --test, -t   Run Razor templating tests");
        Console.WriteLine("  --demo, -d   Generate demo files comparing templates");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  zebra urls.txt");
        Console.WriteLine("  zebra urls.txt my-qr-codes.html");
        Console.WriteLine("  zebra --test");
        Console.WriteLine("  zebra --demo");
    }

    private static string GetOutputFileName(string inputFileName)
    {
        var directory = Path.GetDirectoryName(inputFileName) ?? "";
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
        return Path.Combine(directory, $"{nameWithoutExtension}-qr-codes.html");
    }

    private static void DisplayResults(ProcessResult result)
    {
        Console.WriteLine();
        Console.WriteLine("==========================");
        
        if (result.Success)
        {
            Console.WriteLine("✅ Processing completed successfully!");
            Console.WriteLine($"📊 Total URLs: {result.TotalUrls}");
            Console.WriteLine($"✅ Successful: {result.SuccessfulUrls}");
            
            if (result.FailedUrls > 0)
            {
                Console.WriteLine($"❌ Failed: {result.FailedUrls}");
                
                if (result.Errors.Any())
                {
                    Console.WriteLine("\nErrors:");
                    foreach (var error in result.Errors.Take(5))
                    {
                        Console.WriteLine($"  • {error}");
                    }
                    
                    if (result.Errors.Count > 5)
                    {
                        Console.WriteLine($"  ... and {result.Errors.Count - 5} more errors");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("❌ Processing failed!");
            Console.WriteLine($"Error: {result.ErrorMessage}");
        }

        Console.WriteLine($"⏱️ Processing time: {result.ProcessingTime.TotalSeconds:F2} seconds");
        Console.WriteLine("==========================");
    }
}

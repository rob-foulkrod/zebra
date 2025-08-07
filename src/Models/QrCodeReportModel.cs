namespace Zebra.Models;

public class QrCodeReportModel
{
    public string Title { get; set; } = "QR Code Generator Results";
    public DateTime Timestamp { get; set; }
    public List<UrlQrPair> UrlQrPairs { get; set; } = new();
    public List<UrlQrPair> ValidPairs { get; set; } = new();
    public List<UrlQrPair> InvalidPairs { get; set; } = new();
    public int TotalCount => UrlQrPairs.Count;
    public int ValidCount => ValidPairs.Count;
    public int InvalidCount => InvalidPairs.Count;
    public bool HasInvalidUrls => InvalidPairs.Count > 0;
}

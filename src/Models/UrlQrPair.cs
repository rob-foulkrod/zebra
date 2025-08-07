namespace Zebra.Models;

public class UrlQrPair
{
    public string Url { get; set; } = string.Empty;
    public string Base64QrCode { get; set; } = string.Empty;
    public bool IsValidUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

namespace Zebra.Configuration;

public class ZebraConfig
{
    public string DefaultOutputFileName { get; set; } = "qr-codes.html";
    public string HtmlTitle { get; set; } = "Generated QR Codes";
    public bool IncludeTimestamp { get; set; } = true;
    public bool ShowErrors { get; set; } = true;
}

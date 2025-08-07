using QRCoder;
using System.Drawing;

namespace Zebra.Services;

public class QrCodeService
{
    public string GenerateQrCodeBase64(string url)
    {
        try
        {
            var qrCodeBytes = GenerateQrCodeBytes(url);
            return Convert.ToBase64String(qrCodeBytes);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    private byte[] GenerateQrCodeBytes(string url)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
}

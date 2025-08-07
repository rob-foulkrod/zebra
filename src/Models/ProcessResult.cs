namespace Zebra.Models;

public class ProcessResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TotalUrls { get; set; }
    public int SuccessfulUrls { get; set; }
    public int FailedUrls { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan ProcessingTime { get; set; }
}

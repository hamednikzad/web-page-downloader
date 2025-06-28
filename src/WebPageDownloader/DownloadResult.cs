namespace WebPageDownloader;

public record DownloadResult(string Url, bool IsSucceed, TimeSpan ExecutionTime)
{
    public static DownloadResult DownloadSucceed(string url, TimeSpan executionTime) => new(url, true, executionTime);

    public static DownloadResult DownloadFailed(string url, TimeSpan executionTime) => new(url, false, executionTime);
}
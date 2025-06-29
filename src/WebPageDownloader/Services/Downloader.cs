using System.Diagnostics;
using Serilog;

namespace WebPageDownloader.Services;

public class Downloader(IPathHelper pathHelper, ILogger logger)
{
    public async Task Download(string[] urls)
    {
        logger.Information("The Urls will be downloaded to: {Path}", pathHelper.GetOutputPath());
        
        var tasks = urls.Select(DownloadUrlToFile);

        await foreach (var task in Task.WhenEach(tasks))
        {
            if (task.Result.IsSucceed)
            {
                logger.Information("Download {Url} succeed in {Time}", task.Result.Url, task.Result.ExecutionTime);
            }
        }
    }

    private async Task HandleSuccessResult(string url, byte[] response)
    {
        var filename = pathHelper.GetFileNameFromUrl(url);
        var fStream = File.Create(filename);
        await fStream.WriteAsync(response);
        fStream.Close();
    }

    private async Task<DownloadResult> DownloadUrlToFile(string url)
    {
        var result = await DownloadUrl(url);

        if (result.Data == null) return DownloadResult.DownloadFailed(url, result.ExecutionTime);

        await HandleSuccessResult(url, result.Data);

        return DownloadResult.DownloadSucceed(url, result.ExecutionTime);
    }

    private async Task<(byte[]? Data, TimeSpan ExecutionTime)> DownloadUrl(string url)
    {
        using var client = new HttpClient();

        try
        {
            var startDate = Stopwatch.GetTimestamp();

            using var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsByteArrayAsync();

                return (result, Stopwatch.GetElapsedTime(startDate));
            }

            logger.Error("Download {Url} failed. Http status code: {Code}", url, response.StatusCode);

            return (null, TimeSpan.Zero);
        }
        catch (HttpRequestException e)
        {
            logger.Error(e, "Download {Url} failed", url);

            return (null, TimeSpan.Zero);
        }
    }
}
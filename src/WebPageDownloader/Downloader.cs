using System.Diagnostics;
using System.Text.RegularExpressions;
using Serilog;

namespace WebPageDownloader;

public class Downloader(string path)
{
    public async Task Download(string[] urls)
    {
        var tasks = urls.Select(DownloadUrlToFile);

        await foreach (var task in Task.WhenEach(tasks))
        {
            if (task.Result.IsSucceed)
            {
                Log.Information("Download {Url} succeed in {Time}", task.Result.Url, task.Result.ExecutionTime);
            }
        }
    }

    private async Task HandleSuccessResult(string url, byte[] response)
    {
        var filename = GetFileNameFromUrl(url);
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

            Log.Error("Download {Url} failed. Http status code: {Code}", url, response.StatusCode);

            return (null, TimeSpan.Zero);
        }
        catch (HttpRequestException e)
        {
            Log.Error(e, "Download {Url} failed", url);

            return (null, TimeSpan.Zero);
        }
    }

    private string GetFileNameFromUrl(string url)
    {
        var correctedUrl = Regex.Replace(url, "[\\/:*?<>|\"]", "");
        return Path.Combine(path, correctedUrl + ".html");
    }
}
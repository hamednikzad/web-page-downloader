using Serilog;

namespace WebPageDownloader;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await using var log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        Log.Logger = log;

        var urls = new[] { "https://g.com/hamednikzad/web-page-downloader", "https://github.com/twbs/bootstrap" };
        var tasks = urls.Select(DownloadUrlToFile);

        await foreach (var task in Task.WhenEach(tasks))
        {
            if (task.Result.isSucceed)
            {
                Log.Information("Download {Url} succeed: {Result}", task.Result.url, task.Result.response.Substring(0, 20));
            }
        }
    }

    private static async Task<(string url, bool isSucceed, string response)> DownloadUrlToFile(string url)
    {
        using var client = new HttpClient();
        
        try
        {
            using var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode) return (url, false, "");
            
            var responseBody = await response.Content.ReadAsStringAsync();
            return (url, true, responseBody);

        }
        catch (HttpRequestException e)
        {
            Log.Error(e, "Download {Url} failed", url);
            
            return (url, false, string.Empty);
        }
    }
}
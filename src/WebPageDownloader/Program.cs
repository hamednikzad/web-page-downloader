using Serilog;
using WebPageDownloader.Services;

namespace WebPageDownloader;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigLogging();

        string[] urls;
        try
        {
            urls = new UrlProcessor(args).GetUrlsFromFile();
        }
        catch (Exception e)
        {
            Log.Logger.Error("Failed to get urls from file: {Error}", e.Message);
            return;
        }

        var pathHelper = new PathHelper(Directory.GetCurrentDirectory());
        pathHelper.CleanPath();
        
        // var urls = new[]
        // {
        //     "https://github.com/hamednikzad/web-page-downloader", "https://github.com/twbs/bootstrap",
        //     "https://github.com/some-invalid-url"
        // };

        var downloader = new Downloader(pathHelper, Log.Logger);
        await downloader.Download(urls);
    }
    
    private static void ConfigLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }
}
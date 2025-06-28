using Serilog;

namespace WebPageDownloader;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigLogging();

        CleanPath();
        
        var urls = new[]
        {
            "https://github.com/hamednikzad/web-page-downloader", "https://github.com/twbs/bootstrap",
            "https://github.com/some-invalid-url"
        };

        var downloader = new Downloader(OutputPath);
        await downloader.Download(urls);
    }

    private static readonly string OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");
    private static void CleanPath()
    {
        Directory.CreateDirectory(OutputPath);

        var di = new DirectoryInfo(OutputPath);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }

    private static void ConfigLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        ;
    }
}
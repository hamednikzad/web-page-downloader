using System.Text.RegularExpressions;

namespace WebPageDownloader.Services;

public interface IPathHelper
{
    string GetFileNameFromUrl(string url);
    string GetOutputPath();
}

public class PathHelper(string rootPath, string directoryName = "downloads") : IPathHelper
{
    private readonly string _outputPath = Path.Combine(rootPath, directoryName);
    
    public string GetOutputPath() => _outputPath;
    
    public void CleanPath()
    {
        Directory.CreateDirectory(_outputPath);

        var di = new DirectoryInfo(_outputPath);

        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
    
    public string GetFileNameFromUrl(string url)
    {
        var correctedUrl = Regex.Replace(url, "[\\/:*?<>|\"]", "");
        return Path.Combine(_outputPath, correctedUrl + ".html");
    }
}
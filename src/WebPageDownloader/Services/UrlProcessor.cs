namespace WebPageDownloader.Services;

public class UrlProcessor
{
    private readonly string _filePath;
    public UrlProcessor(string[] commandArgs)
    {
        if(commandArgs.Length != 1)
            throw new ArgumentException("Invalid number of arguments");
        
        _filePath = commandArgs[0];
    }

    public string[] GetUrlsFromFile()
    {
        if(!File.Exists(_filePath))
            throw new FileNotFoundException("File not found");
        
        return File.ReadAllLines(_filePath);
    }
}
using WebPageDownloader.Services;

namespace WebPageDownloader.Test.Services;

public class PathHelperTests
{
    [Theory]
    [InlineData("https://github.com/", "downloads\\httpsgithub.com.html")]
    [InlineData("https://github.com/*:?><", "downloads\\httpsgithub.com.html")]
    public void GetFileNameFromUrl_ShouldReturnCorrectFileName(string url, string expected)
    {
        var sut = new PathHelper("");
        var actual = sut.GetFileNameFromUrl(url);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetOutPath_ShouldReturnCorrectPath()
    {
        var sut = new PathHelper("dir1", "output");

        var expected = "dir1\\output";
        var actual = sut.GetOutputPath();
        
        Assert.Equal(expected, actual);
    }
}
namespace BrowserRouter.Tests;

public class TransformUriTests
{
  private readonly TransformUri _transformUri = new();

  [Fact]
  public void Transform_NoScheme_ReturnsOriginalUrl()
  {
    var url = "http://example.com";
    var browser = new Browser { Name = "Chrome", Location = "/path/to/chrome", Scheme = null };

    var result = _transformUri.Transform(url, browser);

    Assert.Equal(url, result);
  }

  [Fact]
  public void Transform_WithScheme_TransformsUrl()
  {
    var url = "http://example.com";
    var browser = new Browser { Name = "Chrome", Location = "/path/to/chrome", Scheme = "https" };

    var result = _transformUri.Transform(url, browser);

    Assert.Equal("https://example.com/", result);
  }

  [Fact]
  public void Transform_WithDifferentScheme_TransformsUrl()
  {
    var url = "https://example.com";
    var browser = new Browser { Name = "Firefox", Location = "/path/to/firefox", Scheme = "http" };

    var result = _transformUri.Transform(url, browser);

    Assert.Equal("http://example.com/", result);
  }

  [Fact]
  public void Transform_InvalidUrl_ThrowsUriFormatException()
  {
    var url = "invalid-url";
    var browser = new Browser { Name = "Chrome", Location = "/path/to/chrome", Scheme = "https" };

    Assert.Throws<UriFormatException>(() => _transformUri.Transform(url, browser));
  }
}


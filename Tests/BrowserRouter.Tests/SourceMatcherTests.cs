namespace BrowserRouter.Tests;

public class SourceMatcherTests
{
  private readonly SourceMatcher _sourceMatcher = new();

  [Fact]
  public void IsMatch_WindowTitleMatches_ReturnsTrue()
  {
    var source = new Source { WindowTitle = "TestTitle", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }

  [Fact]
  public void IsMatch_WindowTitleDoesNotMatch_ReturnsFalse()
  {
    var source = new Source { WindowTitle = "TestTitle", Browser = "chrome" };
    var windowTitle = "DifferentTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.False(result);
  }

  [Fact]
  public void IsMatch_UrlMatches_ReturnsTrue()
  {
    var source = new Source { Url = "example.com", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }

  [Fact]
  public void IsMatch_UrlDoesNotMatch_ReturnsFalse()
  {
    var source = new Source { Url = "example.com", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://different.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.False(result);
  }

  [Fact]
  public void IsMatch_RegexWindowTitleMatches_ReturnsTrue()
  {
    var source = new Source { WindowTitle = "/Test.*/", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }

  [Fact]
  public void IsMatch_RegexUrlMatches_ReturnsTrue()
  {
    var source = new Source { Url = "/example.*/", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }

  [Fact]
  public void IsMatch_WildcardWindowTitleMatches_ReturnsTrue()
  {
    var source = new Source { WindowTitle = "Test*", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }

  [Fact]
  public void IsMatch_WildcardUrlMatches_ReturnsTrue()
  {
    var source = new Source { Url = "example*", Browser = "chrome" };
    var windowTitle = "TestTitle";
    var uri = new Uri("http://example.com");

    var result = _sourceMatcher.IsMatch(source, windowTitle, uri);

    Assert.True(result);
  }
}
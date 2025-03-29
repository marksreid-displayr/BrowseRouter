using System.Text.Json;

namespace BrowserRouter.Tests;

public class BrowserConverterTests
{
  private readonly JsonSerializerOptions _options = new()
  {
    Converters = { new BrowserConverter() }
  };

  [Fact]
  public void Read_SingleStringValue_ReturnsBrowserList()
  {
    var json = "{\"Chrome\": \"/path/to/chrome\"}";
    var result = JsonSerializer.Deserialize<List<Browser>>(json, _options);

    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("Chrome", result[0].Name);
    Assert.Equal("/path/to/chrome", result[0].Location);
  }

  [Fact]
  public void Read_SingleObjectValue_ReturnsBrowserList()
  {
    var json = "{\"Firefox\": {\"Location\": \"/path/to/firefox\", \"Scheme\": \"https\"}}";
    var result = JsonSerializer.Deserialize<List<Browser>>(json, _options);

    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("Firefox", result[0].Name);
    Assert.Equal("/path/to/firefox", result[0].Location);
    Assert.Equal("https", result[0].Scheme);
  }

  [Fact]
  public void Read_MultipleValues_ReturnsBrowserList()
  {
    var json = "{\"Chrome\": \"/path/to/chrome\", \"Firefox\": {\"Location\": \"/path/to/firefox\", \"Scheme\": \"https\"}}";
    var result = JsonSerializer.Deserialize<List<Browser>>(json, _options);

    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
    Assert.Equal("Chrome", result[0].Name);
    Assert.Equal("/path/to/chrome", result[0].Location);
    Assert.Equal("Firefox", result[1].Name);
    Assert.Equal("/path/to/firefox", result[1].Location);
    Assert.Equal("https", result[1].Scheme);
  }

  [Fact]
  public void Read_InvalidJson_ThrowsJsonException()
  {
    var json = "{\"Chrome\": \"/path/to/chrome\", \"Firefox\": {\"Location\": \"/path/to/firefox\", \"Scheme\": \"https\"";

    Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<Browser>>(json, _options));
  }

  [Fact]
  public void Read_UnexpectedValueKind_ThrowsArgumentOutOfRangeException()
  {
    var json = "{\"Chrome\": 123}";

    Assert.Throws<ArgumentOutOfRangeException>(() => JsonSerializer.Deserialize<List<Browser>>(json, _options));
  }
}
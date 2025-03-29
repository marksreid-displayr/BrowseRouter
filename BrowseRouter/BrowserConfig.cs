using System.Text.Json.Serialization;

namespace BrowseRouter;

public class BrowserConfig
{
  [JsonConverter(typeof(BrowserConverter))]
  public List<Browser>? Browsers { get; set; }
}
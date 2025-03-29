using System.Text.Json.Serialization;

namespace BrowseRouter;

public class Source
{
  [JsonInclude]
  public string? Url { get; set; }

  [JsonInclude]
  public string? WindowTitle { get; set; }

  [JsonInclude]
  public required string? Browser { get; set; }

}
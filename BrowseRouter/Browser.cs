using System.Text.Json.Serialization;

namespace BrowseRouter;

public class Browser
{
  [JsonInclude]
  public string? Name { get; set; }
  [JsonInclude]
  public required string Location { get; set; }
  [JsonInclude]
  public string? Scheme { get; set; }
  [JsonInclude]
  public string[]? Parameters { get; set; }

  public override string ToString() => $"\"{Name}\" (\"{Location}\")";

}
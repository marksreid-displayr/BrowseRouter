using System.Text.Json;
using System.Text.Json.Serialization;

namespace BrowseRouter;

public class BrowserConverter : JsonConverter<List<Browser>>
{
  public override List<Browser> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var result = new List<Browser>();

    using var doc = JsonDocument.ParseValue(ref reader);
    foreach (var property in doc.RootElement.EnumerateObject())
    {
      var key = property.Name;

      switch (property.Value.ValueKind)
      {
        case JsonValueKind.String:
          // If the value is a string, use it as "Value"
          result.Add(new Browser { Name = key, Location = property.Value.GetString() ?? throw new InvalidOperationException() });
          break;
        case JsonValueKind.Object:
        {
          // Deserialize the object and manually set the Name property
          var browser = JsonSerializer.Deserialize<Browser>(property.Value.GetRawText(), options) ?? throw new InvalidOperationException();
          browser.Name = key; // Set key as name
          result.Add(browser);
          break;
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    return result;
  }

  public override void Write(Utf8JsonWriter writer, List<Browser> value, JsonSerializerOptions options)
  {
    throw new NotImplementedException();
  }
}
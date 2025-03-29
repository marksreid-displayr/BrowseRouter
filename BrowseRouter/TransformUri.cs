namespace BrowseRouter;

public class TransformUri : ITransformUri
{
  public string Transform(string url, Browser browser)
  {
    if (string.IsNullOrWhiteSpace(browser.Scheme))
    {
      return url;
    }
    var uri = new Uri(url);
    return new UriBuilder(uri) { Scheme = browser.Scheme, Port = -1 }.Uri.ToString();
  }
}
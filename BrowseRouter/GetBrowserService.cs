using Microsoft.Extensions.Options;

namespace BrowseRouter;

public class GetBrowserService(
  IOptions<BrowserConfig> browserOptions,
  IOptions<Source[]> sourceOptions,
  ISourceMatcher sourceMatcher) : IGetBrowserService
{
  public Browser? GetBrowser(string windowTitle, string url)
  {
    var browsers = browserOptions.Value.Browsers;
    if (browsers == null)
    {
      return null;
    }

    foreach (var source in sourceOptions.Value)
    {
      if (!sourceMatcher.IsMatch(source, windowTitle, new Uri(url))) continue;
      var browser = browsers.FirstOrDefault(b => b.Name == source.Browser);
      if (browser is not null)
      {
        return browser;
      }
    }

    return browsers.FirstOrDefault();
  }
}
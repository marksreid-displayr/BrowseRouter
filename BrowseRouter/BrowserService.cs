using Microsoft.Extensions.Logging;

namespace BrowseRouter;

public class BrowserService(
  ILogger<BrowserService> logger,
  IGetBrowserService getBrowserService,
  IProcessStarter processStarter)
{
  public async Task LaunchAsync(string url, string windowTitle)
  {
    try
    {
      logger.LogInformation(@"Attempting to launch ""{url}"" for ""{windowTitle}""", url, windowTitle);

      var browser = getBrowserService.GetBrowser(windowTitle, url);

      if (browser == null)
      {
        logger.LogInformation("Unable to find a browser matching \"{url}\".", url);
        return;
      }

      var args = (browser.Parameters ?? []).Append(url).ToArray();
      var name = GetAppName(browser.Location);
      var path = Environment.ExpandEnvironmentVariables(browser.Location);

      await processStarter.Start(path, browser.Location, args, name, url);
    }
    catch (Exception e)
    {
      logger.LogInformation(e,"Unexpected exception");
    }
  }

  private static string GetAppName(string path)
  {
    // Get just the app name from the exe at path
    var name = Path.GetFileNameWithoutExtension(path);
    // make first letter uppercase
    name = name[0].ToString().ToUpper() + name[1..];
    return name;
  }
}
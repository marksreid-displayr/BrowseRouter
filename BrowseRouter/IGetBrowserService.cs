namespace BrowseRouter;

public interface IGetBrowserService
{
  Browser? GetBrowser(string windowTitle, string url);
}
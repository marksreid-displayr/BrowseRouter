namespace BrowseRouter;

public interface ISourceMatcher
{
  bool IsMatch(Source source, string windowTitle, Uri url);
}
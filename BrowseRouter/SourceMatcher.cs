using System.Text.RegularExpressions;

namespace BrowseRouter;

public class SourceMatcher : ISourceMatcher
{
  public bool IsMatch(Source source, string windowTitle, Uri uri)
  {
    if (source.WindowTitle is not null)
    {
      if (source.WindowTitle.StartsWith('/') && source.WindowTitle.EndsWith('/'))
      {
        if (!Regex.IsMatch(windowTitle, source.WindowTitle[1..^1]))
        {
          return false;
        }
      }
      else if (!Regex.IsMatch(windowTitle, WildcardEscape(source.WindowTitle)))
      {
        return false;
      }
    }

    if (source.Url is null) return true;

    if (source.Url.StartsWith('/') && source.Url.EndsWith('/'))
    {
      var domain = uri.Authority + uri.AbsolutePath;

      if (!Regex.IsMatch(domain, source.Url[1..^1]))
      {
        return false;
      }
    }
    else if (source.Url.StartsWith('?') && source.Url.EndsWith('?'))
    {
      var domain = uri.Authority + uri.PathAndQuery;

      if (!Regex.IsMatch(domain, source.Url[1..^1]))
      {
        return false;
      }
    }
    else if (!Regex.IsMatch(uri.Authority, WildcardEscape(source.Url)))
    {
      return false;
    }
    return true;
  }

  private static string WildcardEscape(string pattern)
  {
    var regex = Regex.Escape(pattern);
    return $"^{regex.Replace("\\*", ".*")}$";
  }
}
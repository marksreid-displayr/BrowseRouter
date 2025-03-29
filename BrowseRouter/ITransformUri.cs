namespace BrowseRouter;

public interface ITransformUri
{
  string Transform(string url, Browser browser);
}
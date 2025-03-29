namespace BrowseRouter;

public interface IProcessStarter
{
  Task Start(string path, string location, string[] args, string name, string url);
}
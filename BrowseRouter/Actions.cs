using Microsoft.Extensions.Logging;

namespace BrowseRouter;

public class Actions(ILogger<Actions> logger) : IActions
{
  public bool TryRun(Action a)
  {
    try
    {
      a();
      return true;
    }
    catch (Exception e)
    {
      logger.LogInformation($"{e}");
      return false;
    }
  }
}

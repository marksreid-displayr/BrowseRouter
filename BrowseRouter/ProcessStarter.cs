using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BrowseRouter;

public class ProcessStarter(ILogger<ProcessStarter> logger, IActions actions, INotifyService notifier) : IProcessStarter
{
  public async Task Start(string path, string location, string[] args, string name, string url)
  {
    logger.LogInformation("Launching {path} with args \"{args}\"", location, string.Join(' ', args));


    if (!actions.TryRun(() => Process.Start(path, args)))
    {
      await notifier.NotifyAsync($"Error", $"Could not open {name}. Please check the log for more details.");
      return;
    }

    await notifier.NotifyAsync($"Opening {name}", $"URL: {url}");
  }
}
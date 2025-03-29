using System.Text.Json;

namespace BrowseRouter;

using Interop.Win32;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


public static class Program
{

  private static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
      .ConfigureAppConfiguration((context, config) =>
      {
        config.SetBasePath(Directory.GetCurrentDirectory());
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
      })
      .ConfigureServices((context, services) =>
      {
        if (context.Configuration?.GetValue<bool>("Notify") ?? false)
        {
          services.AddSingleton<INotifyService, EmptyNotifyService>();
        }
        else
        {
          services.AddSingleton<INotifyService, NotifyService>();
        }
        services.AddSingleton<DefaultBrowserService>();
        services.AddSingleton<IProcessService, ProcessService>();
        services.AddSingleton<IGetBrowserService, GetBrowserService>();
        services.AddSingleton<BrowserService>();
        services.AddSingleton<ISourceMatcher, SourceMatcher>();
        services.AddSingleton<IProcessStarter, ProcessStarter>();
        services.AddSingleton<IActions, Actions>();

        var jsonSerializerOptions = new JsonSerializerOptions
        {
          Converters = { new BrowserConverter() }
        };

        services.AddSingleton(jsonSerializerOptions);

        var browserConfigSection = context.Configuration?.GetSection("Browsers");
        if (browserConfigSection != null)
        {
          var browsers = JsonSerializer.Deserialize<List<Browser>>(browserConfigSection.Value, jsonSerializerOptions);
          services.Configure<BrowserConfig>(options => options.Browsers = browsers);
        }

        services.Configure<BrowserConfig>(context.Configuration?.GetSection("Browsers") ?? throw new InvalidOperationException("Browser Config section is missing"));
        services.Configure<Source[]>(context.Configuration?.GetSection("Sources") ?? throw new InvalidOperationException("Source Config section is missing"));

        //services.AddSingleton<App>();
      })
      .UseSerilog((context, services, loggerConfiguration) =>
      {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
      });

  private static async Task Main(string[] args)
  {
    Kernel32.AttachToParentConsole();
    var host = CreateHostBuilder(args).Build();
    var config = host.Services.GetRequiredService<IConfiguration>();

    if (ShouldRegister(config, args))
    {
      await RegisterAsync(host);
      return;
    }

    if (ShouldUnregister(config))
    {
      await UnregisterAsync(host);
      return;
    }

    if (ShouldShowHelp(config))
    {
      ShowHelp();
      return;
    }

    var windowTitle = GetWindowTitle(host);
    await LaunchBrowserAsync(host, windowTitle, args.First());
  }

  private static bool ShouldRegister(IConfiguration config, string[] args) =>
    bool.TryParse(config["register"], out var parsedBoolean) ? parsedBoolean : args.Length == 0;

  private static bool ShouldUnregister(IConfiguration config) =>
    bool.TryParse(config["unregister"], out var parsedBoolean) && parsedBoolean;

  private static bool ShouldShowHelp(IConfiguration config) =>
    bool.TryParse(config["help"], out var parsedBoolean) && parsedBoolean;

  private static async Task RegisterAsync(IHost host)
  {
    var defaultBrowserService = host.Services.GetRequiredService<DefaultBrowserService>();
    await defaultBrowserService.RegisterAsync();
  }

  private static async Task UnregisterAsync(IHost host)
  {
    var defaultBrowserService = host.Services.GetRequiredService<DefaultBrowserService>();
    await defaultBrowserService.UnregisterAsync();
  }

  private static string GetWindowTitle(IHost host)
  {
    var processService = host.Services.GetRequiredService<IProcessService>();
    return processService.TryGetParentProcessTitle(out var windowTitle) ? windowTitle : User32.GetActiveWindowTitle();
  }

  private static async Task LaunchBrowserAsync(IHost host, string windowTitle, string url)
  {
    var browserService = host.Services.GetRequiredService<BrowserService>();
    await browserService.LaunchAsync(url,windowTitle);
  }


  //private static async Task Main(string[] args)
  //{
  //  Kernel32.AttachToParentConsole();
  //  var builder = CreateHostBuilder(args);
  //  var host = builder.Build();
  //  var config = host.Services.GetRequiredService<IConfiguration>();

  //  var doRegister = bool.TryParse(config["register"], out var parsedBoolean) ? parsedBoolean : args.Length == 0;
  //  var doUnregister = bool.TryParse(config["unregister"], out parsedBoolean) && parsedBoolean;
  //  var doShowHelp = bool.TryParse(config["help"], out parsedBoolean) && parsedBoolean;
  //  var defaultBrowserService = host.Services.GetRequiredService<DefaultBrowserService>();
  //  var processService = host.Services.GetRequiredService<IProcessService>();
  //  var browserService = host.Services.GetRequiredService<BrowserService>();

  //  if (doRegister)
  //  {
  //    await defaultBrowserService.RegisterAsync();
  //    return;
  //  }
  //  if (doUnregister)
  //  {
  //    await defaultBrowserService.UnregisterAsync();
  //    return;
  //  }

  //  if (doShowHelp)
  //  {
  //    ShowHelp();
  //  }

  //  if (!processService.TryGetParentProcessTitle(out var windowTitle))
  //  {
  //    windowTitle = User32.GetActiveWindowTitle();
  //  }

  //  await browserService.LaunchAsync(windowTitle, args.First());
  //}


  private static void ShowHelp()
  {
    Console.WriteLine
    (
      $@"{nameof(BrowseRouter)}: In Windows, launch a different browser depending on the URL.

     https://github.com/nref/BrowseRouter

     Usage:

      BrowseRouter.exe [-h | --help]
          Show help.

      BrowseRouter.exe
          Automatic registration.
          Same as --register if not already registered, otherwise --unregister.
          If the app has moved or been renamed, updates the existing registration.

      BrowseRouter.exe [-r | --register]
          Register as a web browser, then open Settings.
          The user must choose BrowseRouter as the default browser.
          No need to run as admin.

      BrowseRouter.exe [-u | --unregister]
          Unregister as a web browser.

      BrowseRouter.exe https://example.org/ [...more URLs]
          Launch one or more URLs"
    );
  }
  /*
    private static async Task Main(string[] args)
    {
      Kernel32.AttachToParentConsole();

      if (args.Length == 0)
      {
        await new DefaultBrowserService(new NotifyService(false)).RegisterOrUnregisterAsync();
        return;
      }

      // Process each URL in the arguments list.
      foreach (string arg in args)
      {
        await RunAsync(arg.Trim());
      }
    }

    private static async Task RunAsync(string arg)
    {
      Func<bool> getIsOption = () => arg.StartsWith('-') || arg.StartsWith('/');

      bool isOption = getIsOption();
      while (getIsOption())
      {
        arg = arg[1..];
      }

      if (isOption)
      {
        await RunOption(arg);
        return;
      }

      await LaunchUrlAsyc(arg);
    }

    private static async Task<bool> RunOption(string arg)
    {
      if (string.Equals(arg, "h") || string.Equals(arg, "help"))
      {
        ShowHelp();
        return true;
      }

      if (string.Equals(arg, "r") || string.Equals(arg, "register"))
      {
        await new DefaultBrowserService(new NotifyService(false)).RegisterAsync();
        return true;
      }

      if (string.Equals(arg, "u") || string.Equals(arg, "unregister"))
      {
        await new DefaultBrowserService(new NotifyService(false)).UnregisterAsync();
        return true;
      }

      return false;
    }

    private static async Task LaunchUrlAsyc(string url)
    {
      // Get the window title for whichever application is opening the URL.
      ProcessService processService = new();
      if (!processService.TryGetParentProcessTitle(out string windowTitle))
        windowTitle = User32.GetActiveWindowTitle(); //if it didn't work we get the current foreground window name instead

      ConfigService configService = new();
      Log.Preference = configService.GetLogPreference();

      NotifyPreference notifyPref = configService.GetNotifyPreference();
      INotifyService notifier = notifyPref.IsEnabled switch
      {
        true => new NotifyService(notifyPref.IsSilent),
        false => new EmptyNotifyService()
      };

      await new BrowserService(configService, notifier).LaunchAsync(url, windowTitle);
    }



  */

}
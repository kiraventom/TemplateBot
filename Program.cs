using Serilog;
using Serilog.Events;

namespace TemplateBot;

class Program
{
   private const string PROJECT_NAME = "TemplateBot";

   static async Task Main()
   {
      string projectDirPath;

      if (System.OperatingSystem.IsWindows())
      {
         var appDataDirPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         projectDirPath = Path.Combine(appDataDirPath, PROJECT_NAME);
      }
      else
      {
         var homeDirPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
         projectDirPath = Path.Combine(homeDirPath, $".{PROJECT_NAME}");
      }

      // Create project dir
      Directory.CreateDirectory(projectDirPath);

      // Init logger
      var logsDirPath = Path.Combine(projectDirPath, "logs");
      Directory.CreateDirectory(logsDirPath);
      var logFilePath = Path.Combine(logsDirPath, $"{PROJECT_NAME}.log");
      var logger = InitLogger(logFilePath);

      // Load config
      var configFilePath = Path.Combine(projectDirPath, "config.json");
      var config = Config.Load(configFilePath);

      if (config is null)
      {
         logger.Fatal("Couldn't parse config, exiting");
         return;
      }

      var telegramController = new TelegramController(logger, config.Token);
      telegramController.StartReceiving();

      while (true)
      {
         if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
            return;

         await Task.Delay(10);
      }
   }

   static ILogger InitLogger(string logFilePath)
   {
      var logger = new LoggerConfiguration()
         .WriteTo.File(logFilePath)
         .WriteTo.Console()
         .CreateLogger();

      return logger;
   }
}

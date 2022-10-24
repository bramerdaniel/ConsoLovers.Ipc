namespace ProcessMonitoringServerExtension
{
   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Ipc;
   using ConsoLovers.Toolkit.Ipc.ServerExtension;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddProcessMonitoringServer(config =>
            {
               config.ForName("server")
                  .AddDiagnosticLogging(new ConsoleLogger(ServerLogLevel.Trace));
            })
            .RunAsync();

         Console.ReadLine();
      }
   }
}
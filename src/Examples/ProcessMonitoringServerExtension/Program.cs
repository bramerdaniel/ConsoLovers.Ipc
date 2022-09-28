namespace ProcessMonitoringServerExtension
{
   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Toolkit.ProcessMonitoring.ServerExtension;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<Args>()
            .AddProcessMonitoringServer()
            .RunAsync();
      }
   }
}
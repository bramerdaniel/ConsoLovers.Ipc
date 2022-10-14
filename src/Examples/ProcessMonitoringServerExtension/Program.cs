namespace ProcessMonitoringServerExtension
{
   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Ipc;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddProcessMonitoringServer(config =>
            {
               config.ForName("server")
                  .AddDiagnosticLogging(Console.WriteLine);
            })
            .RunAsync();

         Console.ReadLine();
      }
   }
}
namespace ExecutingClient
{
   using System.Diagnostics;

   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Ipc;

   using ExecutingClient.Client;

   using Microsoft.Extensions.DependencyInjection;

   internal static class Program
   {
      static async Task Main()
      {
         Console.Title = "Remote Execution Client";

         var processStartInfo = new ProcessStartInfo("reServer.exe") { UseShellExecute = true };
         var process = Process.Start(processStartInfo);

         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddService(s => s.AddSingleton(CreateClientFactory))
            .UseMenuWithoutArguments()
            .RunAsync();

         if (process != null)
            await process.WaitForExitAsync();
      }

      static IClientFactory CreateClientFactory(IServiceProvider serviceProvider)
      {
         var clientFactory = IpcClient.CreateClientFactory()
            .ForName("reServer")
            .AddService(s => s.AddSingleton<IRemoteExecutionClient, RemoteExecutionClient>())
            .Build();

         return clientFactory;
      }
   }

}
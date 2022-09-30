namespace ExecutingClient
{
   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Ipc;

   using ExecutingClient.Client;

   using Microsoft.Extensions.DependencyInjection;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddService(s => s.AddSingleton(CreateClientFactory))
            .RunAsync();
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
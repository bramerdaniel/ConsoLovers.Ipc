namespace IpcServerExtension
{
   using ConsoLovers.ConsoleToolkit.Core;
   using ConsoLovers.Ipc;

   using IpcServerExtension.Service;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddIpcServer("abc123")
            .ConfigureIpcServer(x => x.RemoveAspNetCoreLogging())
            .AddGrpcService(typeof(GreeterService))
            .RunAsync();
      }
   }
}
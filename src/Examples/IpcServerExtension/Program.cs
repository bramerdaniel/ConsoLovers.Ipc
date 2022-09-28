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
            .AddIpcServer(c => c.ForName("abc123"), false)
            .AddGrpcService(typeof(GreeterService))
            .RunAsync();
      }
   }
}
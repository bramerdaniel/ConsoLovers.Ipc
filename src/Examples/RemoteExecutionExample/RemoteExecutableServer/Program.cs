namespace RemoteExecutableServer
{
   using ConsoLovers.ConsoleToolkit.Core;

   internal static class Program
   {
      static async Task Main()
      {
         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .AddRemoteExecution()
            .RunAsync();

         Console.ReadLine();
      }
   }
}
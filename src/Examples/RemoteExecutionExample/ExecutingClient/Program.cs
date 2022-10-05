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

         await ConsoleApplication.WithArguments<ApplicationArgs>()
            .UseMenuWithoutArguments()
            .RunAsync();
      }

   }

}
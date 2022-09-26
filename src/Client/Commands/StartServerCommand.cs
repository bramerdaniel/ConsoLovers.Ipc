// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.ProcessMonitoring;

internal class StartServerCommand : IAsyncCommand<StartServerCommand.StartServerArgs>
{
   internal class StartServerArgs
   {
      [Argument("serverPath")]
      [HelpText("The path to the server executable")]
      public string ServerPath { get; set; } = null!;
   }

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      IClientFactory clientFactory = null!;
      try
      {
         var process = Process.Start(new ProcessStartInfo(Arguments.ServerPath) { UseShellExecute = true });
         // // Thread.Sleep(500);
         clientFactory = IpcClient.CreateClientFactory()
            .ForProcess(process)
            .AddProgressClient()
            .Build();
      }
      catch (Exception e)
      {
         Console.ReadLine();
      }

      IProgressClient? progressClient = null;
      try
      { 
         progressClient = clientFactory.CreateProgressClient();
         await progressClient.ConnectAsync(cancellationToken);

         progressClient.ProgressChanged += (s, e) => Console.WriteLine(e.Message);
         await progressClient.WaitForCompletedAsync(cancellationToken);
      }
      finally
      {
         progressClient?.Dispose();
      }
   }

   public StartServerArgs Arguments { get; set; } = null!;
}
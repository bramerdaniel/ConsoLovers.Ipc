// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Exceptions;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.ProcessMonitoring;

internal class StartServerCommand : IAsyncCommand<StartServerCommand.StartServerArgs>
{
   private readonly IConsole console;

   public StartServerCommand(IConsole console)
   {
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #region IAsyncCommand<StartServerArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      console.WriteLine($"Starting server {Arguments.ServerPath}");

      var process = Process.Start(new ProcessStartInfo(Arguments.ServerPath) { UseShellExecute = true });
      if (process == null)
         throw new CommandLineArgumentException("Server could not be started");

      var clientFactory = IpcClient.CreateClientFactory()
         .ForProcess(process)
         .AddProgressClient()
         .Build();

      console.WriteLine($"Created client factory for server {process.ProcessName}.{process.Id}");

      IProgressClient? progressClient = null;
      try
      {
         console.WriteLine("Waiting for ipc server");
         await clientFactory.WaitForServerAsync(cancellationToken);

         progressClient = clientFactory.CreateProgressClient();
         await progressClient.WaitForServerAsync(cancellationToken);

         progressClient.ProgressChanged += (s, e) => Console.WriteLine(e.Message);
         try
         {
            console.WriteLine("Waiting for server result");
            await progressClient.WaitForCompletedAsync(cancellationToken);
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            Console.ReadLine();
         }
      }
      finally
      {
         progressClient?.Dispose();
      }
   }

   public StartServerArgs Arguments { get; set; } = null!;

   #endregion

   internal class StartServerArgs
   {
      #region Public Properties

      [Argument("serverPath")]
      [HelpText("The path to the server executable")]
      public string ServerPath { get; set; } = null!;

      #endregion
   }
}
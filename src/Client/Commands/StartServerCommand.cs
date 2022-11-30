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

internal class StartServerCommand : IAsyncCommand<StartServerCommand.StartServerArgs>, IAsyncMenuCommand
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

      var process = Process.Start(new ProcessStartInfo(Arguments.ServerPath)
      {
         UseShellExecute = true,
         Arguments = Arguments.ServerArguments
      });

      if (process == null)
         throw new CommandLineArgumentException("Server could not be started");
      
      var clientFactory = IpcClient.CreateClientFactory()
         .ForProcess(process)
         .WithLogger(ClientLogLevel.Trace,Log)
         .AddResultClient()
         .Build();

      IResultClient? resultClient = null;
      try
      {
         resultClient = clientFactory.CreateResultClient();
         console.WriteLine("Waiting for ipc server");
         await clientFactory.WaitForServerAsync(cancellationToken);


         try
         {
            console.WriteLine("Waiting for server result");
            var resultInfo = await resultClient.WaitForResultAsync(cancellationToken);
            console.WriteLine($"Result = {resultInfo.ExitCode}, Message = {resultInfo.Message}", ConsoleColor.Green);
         }
         catch (Exception e)
         {
            Console.WriteLine(e);
            Console.ReadLine();
         }
      }
      finally
      {
         resultClient?.Dispose();
      }
   }

   private void Log(string s)
   {
      Console.WriteLine("{0} : {1}", DateTime.Now, s);
   }

   public StartServerArgs Arguments { get; set; } = null!;

   #endregion

   internal class StartServerArgs
   {
      #region Public Properties

      [Argument("serverPath", Index = 0)]
      [HelpText("The path to the server executable")]
      public string ServerPath { get; set; } = null!;
     
      [Argument("arguments", Index = 1)]
      [HelpText("The server arguments")]
      public string ServerArguments { get; set; } = null!;

      #endregion
   }

   public async Task ExecuteAsync(IMenuExecutionContext context, CancellationToken cancellationToken)
   {
      await ExecuteAsync(cancellationToken);
      Console.ReadLine();
   }
}
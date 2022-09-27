// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwaitCancelCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server.Commands;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Server.Annotations;

[UsedImplicitly]
internal class AwaitCancelCommand : ServerCommand, IAsyncCommand<AwaitCancelCommand.RunArgs>
{
   #region Constructors and Destructors

   public AwaitCancelCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<RunArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      await using var server = StartServer(Arguments.ServerName, config => config.AddCancellationHandler());
      var clientTokenSource = new CancellationTokenSource();

      var cancellationHandler = server.GetCancellationHandler();
      cancellationHandler.OnCancellationRequested(ExecuteCancellation);

      try
      {
         Console.WriteLine($"Waiting for {Arguments.Timeout} seconds for client to cancel");
         await Task.Delay(TimeSpan.FromSeconds(Arguments.Timeout), clientTokenSource.Token);
      }
      catch (OperationCanceledException)
      {
         // the client should have cancelled
      }
      finally
      {
         if (!clientTokenSource.IsCancellationRequested)
            Console.WriteLine($"Client did not cancel after {Arguments.Timeout} seconds.", ConsoleColor.Red);
      }

      bool ExecuteCancellation()
      {
         Console.WriteLine("Execution was canceled by client", ConsoleColor.Green);
         clientTokenSource.Cancel();
         return true;
      }
   }

   public RunArgs Arguments { get; set; } = null!;

   #endregion

   [UsedImplicitly]
   internal class RunArgs
   {
      #region Public Properties

      [Argument("name", "server")]
      [HelpText("The name the server should run. If not specified the server will default to <ProcessName>.<ProcessId>")]
      public string? ServerName { get; set; }

      [Argument("timeout", "t")]
      [HelpText("The timeout in seconds, until the server will stop to wait")]
      public int Timeout { get; set; } = 60;

      #endregion
   }
}
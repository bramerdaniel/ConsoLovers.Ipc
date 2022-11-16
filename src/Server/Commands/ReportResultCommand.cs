// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportResultCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server.Commands;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Server.Annotations;

[UsedImplicitly]
internal class ReportResultCommand : ServerCommand, IAsyncCommand<ReportResultCommand.Args>
{
   #region Constants and Fields

   #endregion

   #region Constructors and Destructors

   public ReportResultCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<RunArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      await using var server = StartServer(Arguments.ServerName, c => c.AddResultReporter());
      Console.WriteLine("Waiting for client");
      await server.WaitForClientAsync(cancellationToken);

      var result = server.GetResultReporter();
      result.ReportSuccess();
      //await Task.Delay(3000);
      Environment.Exit(0);
      try
      {
         Console.WriteLine($"Waiting for {Arguments.Timeout} to report the result");
         await Task.Delay(TimeSpan.FromSeconds(Arguments.Timeout), cancellationToken);

         result.ReportResult(Arguments.ExitCode, Arguments.Message);
         Console.WriteLine($"Reported ExitCode={Arguments.ExitCode}, Message={Arguments.Message}", ConsoleColor.Green);
      }
      catch (OperationCanceledException)
      {
         Console.WriteLine("Execution was cancelled", ConsoleColor.Yellow);
      }
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   #region Methods

   #endregion

   [UsedImplicitly]
   internal class Args
   {
      #region Public Properties

      [Argument("timeout", "t")]
      [HelpText("The time to wait until the result is reported")]
      public int Timeout { get; set; } = 5;

      [Argument("name", "server", Index = 0)]
      [HelpText("The name the server should run under")]
      public string? ServerName { get; set; }

      [Argument("exitCode", "exit", "e")]
      [HelpText("The exit code the server will report")]
      public int ExitCode { get; set; } = 0;

      [Argument("message", "m")]
      [HelpText("The message the server will report in the result")]
      public string Message { get; set; } = "OK";

      #endregion
   }
}
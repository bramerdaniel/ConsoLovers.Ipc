// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessMonitoringServerExtension;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

internal class RunCommand : IAsyncCommand<RunCommand.Args>
{
   #region Constants and Fields

   private readonly IIpcServer server;

   private readonly IResultReporter resultReporter;

   private readonly IConsole console;

   #endregion

   #region Constructors and Destructors

   public RunCommand(IIpcServer server, IResultReporter resultReporter, IConsole console)
   {
      this.server = server ?? throw new ArgumentNullException(nameof(server));
      this.resultReporter = resultReporter ?? throw new ArgumentNullException(nameof(resultReporter));
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region IAsyncCommand<ApplicationArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var initialDelay = 15000;
      Console.Title = server.Name;

      console.WriteLine($"Delaying for {initialDelay / 1000 } seconds");
      await Task.Delay(initialDelay, cancellationToken);

      resultReporter.ReportResult(Arguments.ExitCode, Arguments.Message);
      console.WriteLine($"Reported result ExitCode={Arguments.ExitCode}, Message={Arguments.Message}");

      console.WriteLine("Delaying for 2 seconds");
      await Task.Delay(2000, cancellationToken);
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   internal class Args
   {
      #region Public Properties

      [Argument("exitCode", "exit", "e")]
      [HelpText("The exit code the server will report")]
      public int ExitCode { get; set; } = 0;

      [Argument("message", "m")]
      [HelpText("The message the server will report in the result")]
      public string Message { get; set; } = "OK";

      #endregion
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Commands;

using ConsoLovers.ConsoleToolkit.Core;

internal class StartCommand : IAsyncCommand<StartCommand.Args>
{
   #region Constants and Fields

   private readonly IConsole console;

   #endregion

   #region Constructors and Destructors

   public StartCommand(IConsole console)
   {
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      console.WriteLine("Executing started");
      for (int i = 0; i < 50; i++)
      {
         console.Write(".");
         await Task.Delay(30, cancellationToken);
      }

      console.WriteLine();
      console.WriteLine("Execution completed");
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Commands;

using ConsoLovers.ConsoleToolkit.Core;

internal class CountCommand : IAsyncCommand<CountCommand.Args>
{
   #region Constants and Fields

   private readonly IConsole console;

   #endregion

   #region Constructors and Destructors

   public CountCommand(IConsole console)
   {
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      console.WriteLine();

      for (int i = 0; i < Arguments.Number; i++)
      {
         Console.Write($"{i + 1} ");
         await Task.Delay(500, cancellationToken);
      }

      console.WriteLine();
      console.WriteLine("Counting completed");
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   internal class Args
   {
      #region Public Properties

      [Argument("number", "n")]
      [HelpText("The number to count to")]
      public int Number { get; set; } = 5;

      #endregion
   }
}
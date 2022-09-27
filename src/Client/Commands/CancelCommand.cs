// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

internal class CancelCommand : FactoryCommand, IAsyncCommand<CancelCommand.Args>
{
   #region Constructors and Destructors

   public CancelCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var factory = CreateFactory(Arguments.ServerName, config => config.AddCancellationClient());
      var cancellationClient = factory.CreateCancellationClient();
      var cancelled = await cancellationClient.CancelAsync();
      if (cancelled)
      {
         Console.WriteLine($"Cancellation for server {Arguments.ServerName} executed successfully", ConsoleColor.Green);
      }
      else
      {
         Console.WriteLine($"Cancellation for server {Arguments.ServerName} failed", ConsoleColor.Red);
      }
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   internal class Args
   {
      #region Public Properties

      [Argument("server", "name", Index = 0)]
      [HelpText("The name of the server that should be canceled")]
      public string ServerName { get; set; } = null!;

      #endregion
   }
}
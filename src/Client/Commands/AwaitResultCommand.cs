// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwaitResultCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using Client.Annotations;
using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

[UsedImplicitly]
internal class AwaitResultCommand : FactoryCommand, IAsyncCommand<AwaitResultCommand.Args>, IAsyncMenuCommand
{
   #region Constructors and Destructors

   public AwaitResultCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var factory = CreateFactory(Arguments.ServerName, config => config.AddResultClient());
      var resultClient = factory.CreateResultClient();

      Console.WriteLine($"Waiting for server {Arguments.ServerName}");
      await resultClient.WaitForServerAsync(cancellationToken);

      Console.WriteLine($"Waiting for the result of {Arguments.ServerName}");
      var result = await resultClient.WaitForResultAsync(cancellationToken);
      Console.WriteLine($"Server {Arguments.ServerName} has finished", ConsoleColor.Green);
      Console.WriteLine($"ExitCode = {result.ExitCode}", ConsoleColor.Green);
      Console.WriteLine($"Message  = {result.Message}", ConsoleColor.Green);
      if (result.Data.Any())
      {
         Console.WriteLine("--- Data ---", ConsoleColor.Blue);
         foreach (var pair in result.Data)
            Console.WriteLine($" - {pair.Key} = {pair.Value}", ConsoleColor.Blue);
      }
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   internal class Args
   {
      #region Public Properties

      [Argument("server", "name", Index = 0)]
      [HelpText("The name of the server to wait for its result")]
      public string ServerName { get; set; } = null!;

      #endregion
   }

   public async Task ExecuteAsync(IMenuExecutionContext context, CancellationToken cancellationToken)
   {
      await ExecuteAsync(cancellationToken);
      Console.ReadLine();
   }
}
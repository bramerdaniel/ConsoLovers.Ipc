// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwaitProcessCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Globalization;

using Client.Annotations;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.ProcessMonitoring;

[UsedImplicitly]
internal class AwaitProcessCommand : FactoryCommand, IAsyncCommand<AwaitProcessCommand.Args>, IAsyncMenuCommand
{
   #region Constructors and Destructors

   public AwaitProcessCommand(IConsole console)
      : base(console)
   {
   }

   #endregion

   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var factory = CreateFactory(Arguments.ServerName, config =>
      {
         if (Arguments.Culture != null)
            config.WithCulture(new CultureInfo(Arguments.Culture));

         config.AddProgressClient();
      });
      var resultClient = factory.CreateProgressClient();

      Console.WriteLine($"Waiting for server {Arguments.ServerName}");
      await resultClient.WaitForServerAsync(cancellationToken);
      resultClient.ProgressChanged += OnProgressChanged;

      await resultClient.WaitForCompletedAsync(cancellationToken);
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   #region IAsyncMenuCommand Members

   public async Task ExecuteAsync(IMenuExecutionContext context, CancellationToken cancellationToken)
   {
      await ExecuteAsync(cancellationToken);
      Console.ReadLine();
   }

   #endregion

   #region Methods

   private void OnProgressChanged(object? sender, ProgressEventArgs e)
   {
      Console.WriteLine(e.Message);
   }

   #endregion

   internal class Args
   {
      #region Public Properties

      [Argument("culture", "c", Index = 1)]
      [HelpText("The culture to get the progress in")]
      public string? Culture { get; set; } = null;

      [Argument("server", "name", Index = 0)]
      [HelpText("The name of the server to wait for its result")]
      public string ServerName { get; set; } = null!;

      #endregion
   }
}
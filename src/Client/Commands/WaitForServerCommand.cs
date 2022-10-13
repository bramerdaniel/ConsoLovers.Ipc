// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Diagnostics;
using System.Globalization;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Exceptions;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.Clients;

using Spectre.Console;

internal class WaitForServerCommand : IAsyncCommand<WaitForServerCommand.WaitArgs>
{
   #region IAsyncCommand<Args> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      Console.WriteLine($"Waiting for server {Arguments.Name} for {Arguments.Timeout}s");
      var clientFactory = CreateClientFactory();

      try
      {
         //var connectionClient = new SynchronizationClient(clientFactory.ChannelFactory.Channel);
         //await connectionClient.WaitForServerAsync(TimeSpan.FromSeconds(Arguments.Timeout), cancellationToken);
         var progressClient = clientFactory.CreateProgressClient();
         await progressClient.WaitForServerAsync(cancellationToken);

         AnsiConsole.MarkupLine($"[green]Connected to server {clientFactory.ChannelFactory.ServerName} successfully[/]");


      }
      catch (OperationCanceledException)
      {
         AnsiConsole.MarkupLine($"[red]Could not connect to server {clientFactory.ChannelFactory.ServerName} after {Arguments.Timeout}[/]");
      }
   }

   public WaitArgs Arguments { get; set; } = null!;

   #endregion

   #region Methods

   private IClientFactory CreateClientFactory()
   {
      var builder = IpcClient.CreateClientFactory();
      if (!string.IsNullOrWhiteSpace(Arguments.Name))
      {
         return builder.ForName(Arguments.Name)
            .AddProgressClient()
            .Build();
      }

      Process process = FindServerProcess();
      return builder.ForProcess(process)
         .AddProgressClient()
         // .WithDefaultCulture(new CultureInfo("de-DE"))
         .Build();
   }

   private Process FindServerProcess()
   {
      var process = Process.GetProcessesByName("server")
         .FirstOrDefault();
      if (process == null)
         throw new CommandLineArgumentException("Server could not be found");

      return process;
   }

   #endregion

   internal class WaitArgs
   {
      #region Public Properties

      [Argument("name", "n")]
      [HelpText("The name of the process to wait for")]
      public string Name { get; set; } = null!;

      [Argument("timeout", "t")]
      [HelpText("The timeout to wait for the server. The default value is 10s.")]
      public int Timeout { get; set; } = 10;

      #endregion
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExecutingClient.Commands;

using System.Diagnostics;
using System.Text;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using ExecutingClient.Client;

using Microsoft.Extensions.DependencyInjection;

internal class StartServerCommand : IAsyncCommand<StartServerCommand.Args>
{
   private readonly IServiceProvider serviceProvider;

   public StartServerCommand(IServiceProvider serviceProvider)
   {
      this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
   }

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var arguments = ComputeArgs();
      var processStartInfo = new ProcessStartInfo("reServer.exe", arguments) { UseShellExecute = true };
      Process.Start(processStartInfo);
      Console.WriteLine("Started server process");

      var clientFactory = serviceProvider.GetRequiredService<IClientFactory>();
      var executionClient = clientFactory.CreateClient<IRemoteExecutionClient>();
      
      Console.WriteLine("Waiting for server");
      await executionClient.WaitForServerAsync(cancellationToken);
   }

   private string ComputeArgs()
   {
      var builder = new StringBuilder();
      if (!Arguments.Remote)
         return string.Empty;

      builder.Append("Remote ");
      builder.Append($"ServerName={Arguments.ServerName}");
      return builder.ToString();
   }

   internal class Args
   {
      #region Public Properties

      [Argument("server", "sn")]
      [HelpText("The name of the server to execute the command on")]
      public string ServerName { get; set; } = "reServer";

      [Argument("remote")]
      [HelpText("The server will be started for remote execution")]
      public bool Remote { get; set; } = true;

      #endregion
   }

   public Args Arguments { get; set; } = null!;
}
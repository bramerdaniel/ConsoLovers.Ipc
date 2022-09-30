// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExecutingClient.Commands;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using ExecutingClient.Client;

using IpcServerExtension.Grpc;

using Microsoft.Extensions.DependencyInjection;

internal class RemoteExecutionCommand : IAsyncCommand<RemoteExecutionCommand.Args>
{
   #region Constants and Fields
   
   private readonly IConsole console;

   #endregion

   #region Constructors and Destructors

   public RemoteExecutionCommand(IConsole console)
   {
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region IAsyncCommand<ApplicationArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName(Arguments.ServerName)
         .AddService(s => s.AddSingleton<IRemoteExecutionClient, RemoteExecutionClient>())
         .Build();

      var executor = clientFactory.CreateClient<IRemoteExecutionClient>();

      console.WriteLine($"Waiting for server ");
      await executor.WaitForServerAsync(cancellationToken);

      console.WriteLine($"Executing Start");
      await executor.ExecuteCommandAsync(Arguments.CommandName);
      
      console.WriteLine("Delaying for 2 seconds");
      await Task.Delay(2000, cancellationToken);
   }

   private async Task GreetAsync()
   {
      // This call is just for demo form the same process
      // normally another process would do this call
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("abc123")
         .AddService(s => s.AddSingleton<IRemoteExecutionClient, RemoteExecutionClient>())
         .Build();

      var names = new[] { "Robert", "Bob", "Kathy", "Ron", "Melissa", "Jenny", "Wanda", "Paul", "Martha", "Andy" };

      for (int i = 0; i < names.Length; i++)
      {
         var greeterClient = clientFactory.CreateClient<IRemoteExecutionClient>();
         var response = await greeterClient.ExecuteCommandAsync(names[i]);
         Console.WriteLine(response);
         await Task.Delay(300);
      }
   }

   public Args Arguments { get; set; } = null!;

   #endregion

   internal class Args
   {
      #region Public Properties
      
      [Argument("server", "sn")]
      [HelpText("The name of the server to execute the command on")]
      public string ServerName{ get; set; } = "reServer";

      [Argument("command", "c")]
      [HelpText("The name of the command that should be executed")]
      public string CommandName { get; set; } = "Start";

      #endregion
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IpcServerExtension;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using IpcServerExtension.Client;
using IpcServerExtension.Grpc;

using Microsoft.Extensions.DependencyInjection;

internal class RunCommand : IAsyncCommand<RunCommand.Args>
{
   #region Constants and Fields

   private readonly IIpcServer server;

   private readonly IConsole console;

   #endregion

   #region Constructors and Destructors

   public RunCommand(IIpcServer server, IConsole console)
   {
      this.server = server ?? throw new ArgumentNullException(nameof(server));
      this.console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region IAsyncCommand<ApplicationArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      var initialDelay = 1000;
      Console.Title = server.Name;

      console.WriteLine($"Delaying for {initialDelay / 1000 } seconds");
      await Task.Delay(initialDelay, cancellationToken);

      await GreetAsync();

      console.WriteLine("Delaying for 2 seconds");
      await Task.Delay(2000, cancellationToken);
   }

   private async Task GreetAsync()
   {
      // This call is just for demo form the same process
      // normally another process would do this call
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("abc123")
         .AddService(s => s.AddSingleton<IGreeterClient, GreeterClient>())
         .Build();

      var names = new[] { "Robert", "Bob", "Kathy", "Ron", "Melissa", "Jenny", "Wanda", "Paul", "Martha", "Andy" };

      for (int i = 0; i < names.Length; i++)
      {
         var greeterClient = clientFactory.CreateClient<IGreeterClient>();
         var response = await greeterClient.SayHelloAsync(names[i]);
         Console.WriteLine(response);
         await Task.Delay(300);
      }
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
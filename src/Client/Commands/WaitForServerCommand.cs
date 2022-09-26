// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Exceptions;
using ConsoLovers.Ipc;
using ConsoLovers.Ipc.Clients;

internal class WaitForServerCommand : IAsyncCommand<WaitForServerCommand.WaitArgs>
{
   #region IAsyncCommand<WaitArgs> Members

   public async Task ExecuteAsync(CancellationToken cancellationToken)
   {
      Console.WriteLine($"Waiting for server {Arguments.Name}");

      var process = Process.GetProcessesByName(Arguments.Name).FirstOrDefault();
      if (process == null)
         throw new CommandLineArgumentException($"The process {Arguments.Name} could not be found");

      var clientFactory = IpcClient.CreateClientFactory()
         .ForProcess(process)
         .AddProgressClient()
         .Build();

      try
      {
         var timeoutTokenSource = new CancellationTokenSource();
         timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(Arguments.Timeout));

         var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

         var connectionClient = new ConnectionClient(clientFactory.ChannelFactory.Channel);
         await connectionClient.WaitForConnectedAsync(linkedTokenSource.Token);
         Console.WriteLine("Connected");
         Console.ReadLine();
      }
      catch (OperationCanceledException)
      {
         Console.WriteLine($"Could not connect to process {process.ProcessName} after {Arguments.Timeout}");
         Console.ReadLine();
      }
   }

   public WaitArgs Arguments { get; set; } = null!;

   #endregion

   internal class WaitArgs
   {
      #region Public Properties

      [Argument("name", "n")]
      [HelpText("The name of the process to wait for")]
      public string Name { get; set; } = null!;

      [Argument("timeout")]
      [HelpText("The timeout to wait for the server")]
      public int Timeout { get; set; } = 5;

      #endregion
   }
}
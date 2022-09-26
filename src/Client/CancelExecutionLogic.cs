// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelExecutionLogic.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Spectre.Console;

internal class CancelExecutionLogic : IApplicationLogic<ClientArgs>
{
   #region Constants and Fields

   private readonly IClientFactory clientFactory;

   #endregion

   #region Constructors and Destructors

   public CancelExecutionLogic(IClientFactory clientFactory)
   {
      this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
   }

   #endregion

   #region IApplicationLogic<ClientArgs> Members

   public async Task ExecuteAsync(ClientArgs arguments, CancellationToken cancellationToken)
   {
      var cancellationClient = clientFactory.CreateCancellationClient();

      await AnsiConsole.Progress()
         .Columns(new SpinnerColumn(), new PercentageColumn(), new ProgressBarColumn(), new TaskDescriptionColumn())
         .StartAsync(Cancel);

      if (cancellationClient.RequestCancel())
      {
         Console.WriteLine("Cancellation executed successfully");
      }
      else
      {
         Console.WriteLine("Cancellation was rejected");
      }

      await Task.Delay(3000, cancellationToken);
   }

   #endregion

   #region Methods

   private async Task Cancel(ProgressContext context)
   {
      var task = context.AddTask("Time until cancelling");
      for (int i = 0; i <= 100; i++)
      {
         task.Increment(1);
         await Task.Delay(30);
      }
   }

   #endregion
}
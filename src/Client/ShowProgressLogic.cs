// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShowProgressLogic.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Spectre.Console;

internal class ShowProgressLogic : IApplicationLogic<ClientArgs>
{
   #region Constants and Fields

   private readonly IClientFactory clientFactory;

   private IProgressClient? progressClient;

   private IResultClient? resultClient;

   #endregion

   #region Constructors and Destructors

   public ShowProgressLogic(IClientFactory clientFactory)
   {
      this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
   }

   #endregion

   #region IApplicationLogic<ClientArgs> Members

   public async Task ExecuteAsync(ClientArgs arguments, CancellationToken cancellationToken)
   {
      Console.Title = $"client [{clientFactory.ChannelFactory.Address}]";
      progressClient = clientFactory.CreateClient<IProgressClient>();
      resultClient = clientFactory.CreateClient<IResultClient>();

      await AnsiConsole.Progress()
         .Columns(new ProgressColumn[] { new SpinnerColumn(), new PercentageColumn(), new ProgressBarColumn(), new TaskDescriptionColumn() })
         .StartAsync(Update);

      Console.Clear();
      var resultInfo = await resultClient.WaitForResultAsync();
      Console.WriteLine($"{clientFactory.ChannelFactory.Address} exited with code {resultInfo.ExitCode}");

      Console.ReadLine();
   }

   #endregion

   #region Methods

   private async Task Update(ProgressContext progressContext)
   {
      var progress = progressContext.AddTask(clientFactory.ChannelFactory.Address);
      progressClient.ProgressChanged += OnProgressChanged;

      void OnProgressChanged(object? sender, ProgressEventArgs e)
      {
         progress.Value = e.Percentage;
         progress.Description = e.Message;
      }

      await resultClient.WaitForResultAsync();
   }

   #endregion
}
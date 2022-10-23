// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class ProgressClient : ConfigurableClient<ProgressService.ProgressServiceClient>, IProgressClient
{


   #region Constants and Fields

   private ClientState state = ClientState.Uninitialized;

   #endregion

   #region Public Events

   /// <summary>Occurs when the reported progress of the server has changed.</summary>
   public event EventHandler<ProgressEventArgs>? ProgressChanged;

   /// <summary>Occurs when <see cref="State"/> property has changed.</summary>
   public event EventHandler<StateChangedEventArgs>? StateChanged;

   #endregion

   #region IProgressClient Members

   /// <summary>Gets the state state of the client.</summary>
   public ClientState State
   {
      get => state;
      private set
      {
         var oldState = state;
         if (state == value)
            return;

         state = value;
         StateChanged?.Invoke(this, new StateChangedEventArgs(oldState, state));
      }
   }

   /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
   public void Dispose()
   {
      // Noting to dispose
   }

   /// <summary>Gets the exception that occurred when the state is failed.</summary>
   public Exception? Exception { get; private set; }

   public Task WaitForCompletedAsync()
   {
      return WaitForCompletedAsync(CancellationToken.None);
   }

   public Task WaitForCompletedAsync(CancellationToken cancellationToken)
   {
      return Task.Run(() => WaitForFinished(cancellationToken));
   }

   #endregion

   #region Methods

   protected override void OnConfigured()
   {
      Task.Run(ConnectAsync);
   }

   protected override Task OnServerConnectedAsync()
   {
      State = ClientState.Active;
      return Task.CompletedTask;
   }

   private async Task ConnectAsync()
   {
      State = ClientState.Connecting;
      await WaitForServerAsync(CancellationToken.None);
      await UpdateProgressAsync();
   }

   private async Task UpdateProgressAsync()
   {
      try
      {
         var streamingCall = ServiceClient.ProgressChanged(new ProgressChangedRequest(), CreateLanguageHeader());
         
         while (await streamingCall.ResponseStream.MoveNext(CancellationToken.None))
         {
            var currentResponse = streamingCall.ResponseStream.Current;
            ProgressChanged?.Invoke(this, new ProgressEventArgs
            {
               Percentage = currentResponse.Progress.Percentage, 
               Message = currentResponse.Progress.Message
            });
         }

         State = ClientState.Closed;
      }
      catch (Exception ex)
      {
         State = ClientState.Failed;
         Exception = ex;
      }
   }

   private void WaitForFinished(CancellationToken cancellationToken)
   {
      var resetEventSlim = new ManualResetEventSlim();

      try
      {
         StateChanged += OnStateChanged;
         CheckForFinished(State);

         resetEventSlim?.Wait(cancellationToken);
      }
      catch (OperationCanceledException)
      {
         // Waiting was canceled
      }
      finally
      {
         StateChanged -= OnStateChanged;
      }

      void OnStateChanged(object? sender, StateChangedEventArgs e)
      {
         CheckForFinished(e.NewState);
      }

      void CheckForFinished(ClientState stateToCheck)
      {
         if (resetEventSlim == null)
            return;

         if (stateToCheck == ClientState.Closed || stateToCheck == ClientState.Failed)
         {
            resetEventSlim.Set();
            resetEventSlim.Dispose();
            resetEventSlim = null;
         }
      }
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Progress;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.Grpc;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class ProgressClient : IProgressClient
{
   #region Constants and Fields

   private Task? progressTask;

   private Grpc.ProgressService.ProgressServiceClient? serviceClient;

   private ClientState state = ClientState.Uninitialized;

   #endregion

   #region Public Events

   public event EventHandler<ProgressEventArgs>? ProgressChanged;

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

   public void Configure(IClientConfiguration configuration)
   {
      serviceClient = new Grpc.ProgressService.ProgressServiceClient(configuration.Channel);
      State = ClientState.Connecting;
      progressTask = Task.Run(ReadProgress);
   }

   /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
   public void Dispose()
   {
      progressTask?.Dispose();
   }

   public Task WaitForCompletedAsync()
   {
      return WaitForCompletedAsync(CancellationToken.None);
   }

   /// <summary>Gets the exception that occurred when the state is failed.</summary>
   public Exception? Exception { get; private set; }

   #endregion

   #region Properties

   private Grpc.ProgressService.ProgressServiceClient ServiceClient =>
      serviceClient ?? throw new InvalidOperationException("Service client is not initialized");

   #endregion

   #region Public Methods and Operators

   public Task WaitForCompletedAsync(CancellationToken cancellationToken)
   {
      return Task.Run(() => WaitForFinished(cancellationToken));
   }

   #endregion

   #region Methods

   private async Task ReadProgress()
   {
      try
      {
         var streamingCall = ServiceClient.ProgressChanged(new ProgressChangedRequest());
         State = ClientState.Active;

         while (await streamingCall.ResponseStream.MoveNext(CancellationToken.None))
         {
            var currentResponse = streamingCall.ResponseStream.Current;
            ProgressChanged?.Invoke(this,
               new ProgressEventArgs { Percentage = currentResponse.Progress.Percentage, Message = currentResponse.Progress.Message });
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
         resetEventSlim.Wait(cancellationToken);
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
         if (stateToCheck == ClientState.Closed || stateToCheck == ClientState.Failed)
         {
            resetEventSlim.Set();
            resetEventSlim.Dispose();
         }
      }
   }

   #endregion
}
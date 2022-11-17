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

   private readonly CancellationTokenSource clientDisposedSource;

   private readonly ManualResetEventSlim progressTaskWaitHandle;

   private ClientState state = ClientState.NotConnected;

   private Task? synchronizeTask;

   #endregion

   #region Constructors and Destructors

   public ProgressClient()
   {
      clientDisposedSource = new CancellationTokenSource();
      progressTaskWaitHandle = new ManualResetEventSlim();
   }

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
      clientDisposedSource.Cancel();
      clientDisposedSource.Dispose();
   }

   /// <summary>Gets the exception that occurred when the state is failed.</summary>
   public Exception? Exception { get; private set; }

   public async Task WaitForCompletedAsync(CancellationToken cancellationToken)
   {
      if (!progressTaskWaitHandle.IsSet || synchronizeTask == null)
         progressTaskWaitHandle.Wait(cancellationToken);

      await ProgressTask.WaitAsync(cancellationToken);
   }

   #endregion

   #region Public Properties

   public Task ProgressTask
   {
      get => synchronizeTask ?? throw new InvalidOperationException("ProgressTask was not created yet");
      set
      {
         if (synchronizeTask != null)
            throw new InvalidOperationException("ProgressTask already specified");

         synchronizeTask = value;
         progressTaskWaitHandle.Set();
      }
   }

   #endregion

   #region Methods

   protected override void OnConfigured()
   {
      State = ClientState.Connecting;
      SynchronizationClient.SynchronizeAsync(clientDisposedSource.Token, OnConnectionEstablished);
   }

   private void OnConnectionEstablished(CancellationToken cancellationToken)
   {
      State = ClientState.Connected;
      var progressChangedCall = ServiceClient.ProgressChanged(new ProgressChangedRequest(), CreateLanguageHeader());
      ProgressTask = UpdateProgressAsync(progressChangedCall);
   }

   private async Task UpdateProgressAsync(AsyncServerStreamingCall<ProgressChangedResponse> progressCall)
   {
      try
      {
         while (await progressCall.ResponseStream.MoveNext(CancellationToken.None))
         {
            var currentResponse = progressCall.ResponseStream.Current;
            ProgressChanged?.Invoke(this,
               new ProgressEventArgs { Percentage = currentResponse.Progress.Percentage, Message = currentResponse.Progress.Message });
         }

         State = ClientState.ConnectionClosed;
      }
      catch (RpcException ex)
      {
         throw IpcException.FromRpcException(ex);
      }
      catch (Exception ex)
      {
         State = ClientState.ConnectionClosed;
         Exception = ex;
      }
   }

   #endregion
}
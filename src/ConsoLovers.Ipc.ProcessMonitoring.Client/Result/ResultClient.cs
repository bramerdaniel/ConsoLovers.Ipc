// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ResultClient : ConfigurableClient<ResultService.ResultServiceClient>, IResultClient
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resultWaitHandle;

   private ResultInfo? result;

   private ClientState state = ClientState.Uninitialized;

   private Task? synchronizeTask;

   private readonly ManualResetEventSlim synchronizeTaskWaitHandle;

   private readonly CancellationTokenSource clientDisposedSource;

   #endregion

   #region Constructors and Destructors

   public ResultClient()
   {
      clientDisposedSource = new CancellationTokenSource();
      resultWaitHandle = new ManualResetEventSlim();
      synchronizeTaskWaitHandle = new ManualResetEventSlim();
   }

   #endregion

   #region Public Events

   public event EventHandler<ResultEventArgs>? ResultChanged;

   /// <summary>Occurs when <see cref="State"/> property has changed.</summary>
   public event EventHandler<StateChangedEventArgs>? StateChanged;

   #endregion

   #region IResultClient Members

   /// <summary>Gets the <see cref="Exception"/> that occurred when the state goes to <see cref="ClientState.Failed"/>.</summary>
   public Exception? Exception { get; private set; }

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
   
   public async Task<ResultInfo> WaitForResultAsync(CancellationToken cancellationToken)
   {
      if (result != null)
         return result;

      if (!synchronizeTaskWaitHandle.IsSet || synchronizeTask == null)
         synchronizeTaskWaitHandle.Wait(cancellationToken);

      await SynchronizationTask.WaitAsync(cancellationToken);
      return Result;
   }

   public Task<ResultInfo> WaitForResultAsync()
   {
      return WaitForResultAsync(CancellationToken.None);
   }

   public void Dispose()
   {
      clientDisposedSource.Cancel();
      clientDisposedSource.Dispose();
   }

   #endregion

   #region Properties

   private ResultInfo Result
   {
      get => result ??= new ResultInfo(ExitCode: int.MaxValue, Message: "Result not computed yet", Data: new Dictionary<string, string>());
      set
      {
         result = value;
         resultWaitHandle.Set();
         ResultChanged?.Invoke(this, new ResultEventArgs(result.ExitCode, result.Message));
      }
   }

   public Task SynchronizationTask
   {
      get => synchronizeTask ?? throw new InvalidOperationException("SynchronizationTask was not created yet");
      set
      {
         if (synchronizeTask != null)
            throw new InvalidOperationException("SynchronizeTask already specified");

         synchronizeTask = value;
         synchronizeTaskWaitHandle.Set();
      }
   }

   #endregion

   #region Methods

   protected override void OnConfigured()
   {
      SynchronizationClient.SynchronizeAsync(clientDisposedSource.Token, OnConnectionEstablished);
   }

   private void OnConnectionEstablished(CancellationToken cancellationToken)
   {
      var resultChangedStream = ServiceClient.ResultChanged(new ResultChangedRequest { ClientName = SynchronizationClient.Id }, CreateLanguageHeader());
      SynchronizationTask = ListenToResult(resultChangedStream);
   }

   private async Task ListenToResult(AsyncServerStreamingCall<ResultChangedResponse> resultChanged)
   {
      try
      {
         if (await resultChanged.ResponseStream.MoveNext(CancellationToken.None))
         {
            var response = resultChanged.ResponseStream.Current;
            Result = new ResultInfo(ExitCode: response.ExitCode, Message: response.Message, Data: response.Data);
         }

         State = ClientState.Closed;
      }
      catch (RpcException ex)
      {
         // This happens when the server was available and is disposed without reporting any results
         State = ClientState.Failed;
         Exception = ex;
      }
      catch (Exception ex)
      {
         State = ClientState.Failed;
         Exception = ex;
         result = new ResultInfo(ExitCode: int.MaxValue, Message: ex.Message, Data: new Dictionary<string, string>());
      }
   }

   #endregion
}
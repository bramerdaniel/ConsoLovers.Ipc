// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ResultClient : ConfigurableClient<ResultService.ResultServiceClient>, IResultClient
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resultWaitHandle;

   private ResultInfo? result;

   private ClientState state = ClientState.NotConnected;

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
      State = ClientState.Connected;
   }

   private async Task ListenToResult(AsyncServerStreamingCall<ResultChangedResponse> resultChanged)
   {
      try
      {
         if (await resultChanged.ResponseStream.MoveNext())
         {
            var response = resultChanged.ResponseStream.Current;
            Result = new ResultInfo(ExitCode: response.ExitCode, Message: response.Message, Data: response.Data);
         }
      }
      catch (RpcException ex)
      {
         throw IpcException.FromRpcException(ex);
      }
      catch (Exception ex)
      {
         throw new IpcException("Unknown error while waiting for the result", ex);
      }
      finally
      {
         State = ClientState.ConnectionClosed;
      }
   }

   #endregion
}
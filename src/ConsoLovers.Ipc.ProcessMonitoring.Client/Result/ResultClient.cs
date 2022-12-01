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
public class ResultClient : ConfigurableClient<ResultService.ResultServiceClient>, IResultClient, ISynchronizedClient
{
   #region Constants and Fields

   private readonly CancellationTokenSource clientDisposedSource;

   private readonly IClientLogger logger;

   private readonly ManualResetEventSlim resultWaitHandle;

   private readonly ManualResetEventSlim synchronizeTaskWaitHandle;

   private ResultInfo? result;

   private ClientState state = ClientState.NotConnected;

   private Task? synchronizeTask;

   #endregion

   #region Constructors and Destructors

   public ResultClient(IClientLogger logger)
   {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

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

      await ResultWaitingTask.WaitAsync(cancellationToken);
      return Result;
   }

   public void Dispose()
   {
      clientDisposedSource.Cancel();
      clientDisposedSource.Dispose();
   }

   #endregion

   #region ISynchronizedClient Members

   public string Id => $"{SynchronizationClient.Id}.{nameof(ResultClient)}";

   public void OnConnectionConfirmed(CancellationToken cancellationToken)
   {
      logger.Info($"{Id} has connected to server successfully.");
   }

   public void OnConnectionAborted(CancellationToken cancellationToken)
   {
      logger.Info($"{Id} could not connect to server.");
   }

   public void OnConnectionEstablished(CancellationToken cancellationToken)
   {
      var resultChangedStream =
         ServiceClient.ResultChanged(new ResultChangedRequest { ClientName = SynchronizationClient.Id }, CreateLanguageHeader());
      ResultWaitingTask = Task.Run(() => ListenToResult(resultChangedStream , cancellationToken), cancellationToken);
      State = ClientState.Connected;
   }

   #endregion

   #region Properties

   private Task ConnectionTask { get; set; }

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

   /// <summary>Gets or sets the <see cref="Task"/> that is created for waiting for the result.</summary>
   private Task ResultWaitingTask
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
      ConnectionTask = Task.Run(() => SynchronizationClient.SynchronizeAsync(clientDisposedSource.Token, this), clientDisposedSource.Token);
   }

   private async Task ListenToResult(AsyncServerStreamingCall<ResultChangedResponse> resultChanged, CancellationToken cancellationToken)
   {
      try
      {
         logger.Debug($"{Id} is waiting for the server result");
         if (await resultChanged.ResponseStream.MoveNext(cancellationToken))
         {
            var response = resultChanged.ResponseStream.Current;
            logger.Debug($"{Id} got result from server");

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
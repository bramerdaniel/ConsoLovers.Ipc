﻿// --------------------------------------------------------------------------------------------------------------------
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
   
   #endregion

   #region Constructors and Destructors

   public ResultClient()
   {
      resultWaitHandle = new ManualResetEventSlim();
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

      await Task.Run(() => WaitForFinished(cancellationToken));
      return Result;
   }

   public Task<ResultInfo> WaitForResultAsync()
   {
      return WaitForResultAsync(CancellationToken.None);
   }

   public void Dispose()
   {
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

   #endregion

   #region Methods

   protected override void OnConfigured()
   {
      Task.Run(ConnectAsync);
   }

   private async Task ConnectAsync()
   {
      State = ClientState.Connecting;
      await WaitForServerAsync(CancellationToken.None);
      await WaitForResult();
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

   private async Task WaitForResult()
   {
      try
      {
         var streamingCall = ServiceClient.ResultChanged(new ResultChangedRequest());
         await WaitForResult(streamingCall);

         State = ClientState.Closed;
      }
      catch (Exception ex)
      {
         State = ClientState.Failed;
         Exception = ex;
      }
   }

   private async Task WaitForResult(AsyncServerStreamingCall<ResultChangedResponse> changed)
   {
      try
      {
         if (await changed.ResponseStream.MoveNext(CancellationToken.None))
         {
            var response = changed.ResponseStream.Current;
            Result = new ResultInfo(ExitCode: response.ExitCode, Message: response.Message, Data: response.Data);
         }

         State = ClientState.Closed;
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
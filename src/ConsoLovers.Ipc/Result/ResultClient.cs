// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Result;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class ResultClient : IResultClient
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resultWaitHandle;

   private ResultInfo? result;

   private Grpc.ResultService.ResultServiceClient? serviceClient;

   private Task<ResultInfo>? waitingTask;

   #endregion

   #region Constructors and Destructors

   public ResultClient()
   {
      resultWaitHandle = new ManualResetEventSlim();
   }

   #endregion

   #region Public Events

   public event EventHandler<ResultEventArgs>? ResultChanged;

   #endregion

   #region IResultClient Members

   public async Task<ResultInfo> WaitForResultAsync()
   {
      if (result != null)
         return result;

      return await GetOreCreateWaitingTask();
   }

   public void Configure(IClientConfiguration configuration)
   {
      serviceClient = new Grpc.ResultService.ResultServiceClient(configuration.Channel);
      GetOreCreateWaitingTask();
   }

   public void Dispose()
   {
      waitingTask?.Dispose();
   }

   #endregion

   #region Properties

   private ResultInfo Result
   {
      get => result;
      set
      {
         result = value;
         resultWaitHandle.Set();
         ResultChanged?.Invoke(this, new ResultEventArgs(result.ExitCode, result.Message));
      }
   }

   #endregion

   #region Methods

   private Task<ResultInfo> GetOreCreateWaitingTask()
   {
      if (waitingTask == null)
      {
         var streamingCall = GetServiceClient().ResultChanged(new ResultChangedRequest());
         waitingTask = Task.Run(() => WaitForResult(streamingCall));
      }

      return waitingTask;
   }

   private Grpc.ResultService.ResultServiceClient GetServiceClient()
   {
      if (serviceClient == null)
         throw new InvalidOperationException("Not initialized yet");

      return serviceClient;
   }

   private async Task<ResultInfo> WaitForResult(AsyncServerStreamingCall<ResultChangedResponse> changed)
   {
      if (await changed.ResponseStream.MoveNext(CancellationToken.None))
      {
         var response = changed.ResponseStream.Current;
         Result = new ResultInfo { ExitCode = response.ExitCode, Message = response.Message };
      }

      return Result;
   }

   #endregion
}
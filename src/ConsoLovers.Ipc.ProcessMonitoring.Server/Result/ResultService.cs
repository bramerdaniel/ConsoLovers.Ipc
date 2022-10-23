// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Result;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

using Microsoft.Extensions.Hosting;

internal class ResultService : Grpc.ResultService.ResultServiceBase
{
   #region Constants and Fields

   private readonly ResultReporter resultReporter;

   private readonly IServerLogger logger;

   private readonly IHostApplicationLifetime hostLifetime;

   #endregion

   #region Constructors and Destructors

   public ResultService(ResultReporter resultReporter, IServerLogger logger, IHostApplicationLifetime hostLifetime)
   {
      this.resultReporter = resultReporter ?? throw new ArgumentNullException(nameof(resultReporter));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      this.hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
   }

   #endregion

   #region Public Methods and Operators

   public override async Task ResultChanged(ResultChangedRequest request, IServerStreamWriter<ResultChangedResponse> responseStream,
      ServerCallContext context)
   {
      var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, hostLifetime.ApplicationStopping);

      try
      {
         logger.Debug("Result handler was attached");
         var resultInfo = await resultReporter.GetResultAsync(tokenSource.Token);
         
         logger.Debug("Result was available in result service");
         var response = CreateResponse(resultInfo);
         await responseStream.WriteAsync(response, tokenSource.Token);
      }
      catch (OperationCanceledException)
      {
         logger.Debug("Waiting for result was canceled");
      }
   }

   private static ResultChangedResponse CreateResponse(ResultInfo resultInfo)
   {
      var response = new ResultChangedResponse { ExitCode = resultInfo.ExitCode, Message = resultInfo.Message };
      if (resultInfo.Data.Count > 0)
         response.Data.Add(resultInfo.Data);
      return response;
   }

   #endregion
}
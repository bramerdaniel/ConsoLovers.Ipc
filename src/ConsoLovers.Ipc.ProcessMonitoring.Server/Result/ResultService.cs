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
      logger.Debug("ResultService was created");
   }

   #endregion

   #region Public Methods and Operators

   public override async Task ResultChanged(ResultChangedRequest request, IServerStreamWriter<ResultChangedResponse> responseStream,
      ServerCallContext context)
   {
      var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, hostLifetime.ApplicationStopping);
      var culture = context.GetCulture();

      // we register every client that is interested in the result 
      var disposable = resultReporter.RegisterRequest(request.ClientName);

      try
      {
         var response = await resultReporter.GetResultAsync(culture, tokenSource.Token);
         logger.Debug("Reported result is available in result service");

         await responseStream.WriteAsync(response, tokenSource.Token);
      }
      catch (OperationCanceledException)
      {
         if (context.CancellationToken.IsCancellationRequested)
         {
            logger.Debug($"ResultRequest for client '{request.ClientName}' was canceled");
         }
         else
         {
            logger.Debug($"ResultRequest for client '{request.ClientName}' was canceled as the server application is shutting down");
            throw new RpcException(new Status(StatusCode.Aborted, "Server was shut down"));
         }
      }
      finally
      {
         //  now the registered client got the result,
         // or is no longer interested in it. 
         disposable.Dispose();
         tokenSource.Dispose();
      }
   }

   #endregion
}
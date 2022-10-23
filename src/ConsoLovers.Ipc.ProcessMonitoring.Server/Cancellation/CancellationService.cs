// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelationService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Cancellation;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

internal class CancellationService : Grpc.CancellationService.CancellationServiceBase
{
   private readonly CancellationHandler cancellationHandler;

   private readonly IServerLogger logger;

   public CancellationService(CancellationHandler cancellationHandler, IServerLogger logger)
   {
      this.cancellationHandler = cancellationHandler ?? throw new ArgumentNullException(nameof(cancellationHandler));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   public override Task<RequestCancelResponse> RequestCancel(RequestCancelRequest request, ServerCallContext context)
   {
      logger.Debug("Cancellation was requested by client");
      var accepted = cancellationHandler.RequestCancel();
      return Task.FromResult(new RequestCancelResponse { CancelationAccepted = accepted });
   }
}
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

   public CancellationService(CancellationHandler cancellationHandler)
   {
      this.cancellationHandler = cancellationHandler ?? throw new ArgumentNullException(nameof(cancellationHandler));
   }

   public override Task<RequestCancelResponse> RequestCancel(RequestCancelRequest request, ServerCallContext context)
   {
      var accepted = cancellationHandler.RequestCancel();
      return Task.FromResult(new RequestCancelResponse { CancelationAccepted = accepted });
   }
}
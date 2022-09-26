// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Cancellation;

using ConsoLovers.Ipc;
using ConsoLovers.Ipc.Grpc;

internal class CancellationClient : ConfigurableClient<Grpc.CancellationService.CancellationServiceClient>, ICancellationClient
{
   public bool RequestCancel()
   {
      return ServiceClient.RequestCancel(new RequestCancelRequest()).CancelationAccepted;
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

using ConsoLovers.Ipc.Grpc;

internal class CancellationClient : ConfigurableClient<Grpc.CancellationService.CancellationServiceClient>, ICancellationClient
{
   #region ICancellationClient Members

   /// <summary>Cancels the server execution and returns the if the cancel request was accepted.</summary>
   /// <returns>True if the cancellation was accepted, otherwise false</returns>
   public bool Cancel()
   {
      return ServiceClient.RequestCancel(new RequestCancelRequest()).CancelationAccepted;
   }

   /// <summary>Cancels the server execution and returns the if the cancel request was accepted.</summary>
   /// <returns>True if the cancellation was accepted, otherwise false</returns>
   public async Task<bool> CancelAsync()
   {
      var response = await ServiceClient.RequestCancelAsync(new RequestCancelRequest());
      return response.CancelationAccepted;
   }

   #endregion
}
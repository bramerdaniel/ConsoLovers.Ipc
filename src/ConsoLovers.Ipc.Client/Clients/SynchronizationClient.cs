// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Clients;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;
using global::Grpc.Net.Client;

/// <summary>Helper service to check if a specific server is available</summary>
/// <seealso cref="ConsoLovers.Ipc.ISynchronizationClient"/>
public class SynchronizationClient : ISynchronizationClient
{
   #region Constants and Fields

   private readonly SynchronizatioService.SynchronizatioServiceClient connectionService;

   #endregion

   #region Constructors and Destructors

   public SynchronizationClient(GrpcChannel channel)
   {
      if (channel == null)
         throw new ArgumentNullException(nameof(channel));

      connectionService = new SynchronizatioService.SynchronizatioServiceClient(channel);
   }

   #endregion

   #region ISynchronizationClient Members

   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      while (true)
      {
         cancellationToken.ThrowIfCancellationRequested();

         try
         {
            await connectionService.ConnectAsync(new ConnectRequest());
            return;
         }
         catch (RpcException)
         {
            await Task.Delay(100, cancellationToken);
         }
      }
   }

   public async Task WaitForServerAsync(TimeSpan timeout)
   {
      using var tokenSource = new CancellationTokenSource();
      tokenSource.CancelAfter(timeout);

      await WaitForServerAsync(tokenSource.Token);
   }

   /// <summary>Waits for the specified timeout for server to be available.</summary>
   /// <param name="timeout">The timeout.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   public async Task WaitForServerAsync(TimeSpan timeout, CancellationToken cancellationToken)
   {
      using var timeoutSource = new CancellationTokenSource();
      timeoutSource.CancelAfter(timeout);

      using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutSource.Token);

      await WaitForServerAsync(linkedTokenSource.Token);
   }

   #endregion
}
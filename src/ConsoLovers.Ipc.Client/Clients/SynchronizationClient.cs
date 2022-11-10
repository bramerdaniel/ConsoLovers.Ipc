// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Clients;

using System.Diagnostics;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;
using global::Grpc.Net.Client;

/// <summary>Helper service to check if a specific server is available</summary>
/// <seealso cref="ConsoLovers.Ipc.ISynchronizationClient"/>
public class SynchronizationClient : ISynchronizationClient
{
   private readonly IClientLogger logger;

   #region Constants and Fields

   private readonly SynchronizatioService.SynchronizatioServiceClient connectionService;

   private bool wasConnected;

   private readonly string clientName;

   #endregion

   #region Constructors and Destructors

   public SynchronizationClient(GrpcChannel channel, IClientLogger logger)
   {
      if (channel == null)
         throw new ArgumentNullException(nameof(channel));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

      clientName = CreateClientName();
      logger.Debug($"SynchronizationClient with name {clientName} was created");
      connectionService = new SynchronizatioService.SynchronizatioServiceClient(channel);
      wasConnected = false;
   }

   #endregion

   #region ISynchronizationClient Members

   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      while (!wasConnected)
      {
         cancellationToken.ThrowIfCancellationRequested();

         try
         {
            await connectionService.ConnectAsync(new ConnectRequest { ClientName = clientName });
            logger.Debug($"{nameof(ISynchronizationClient)} could connect successfully");
            wasConnected = true;
            return;
         }
         catch (RpcException e)
         {
            logger.Trace($"{nameof(ISynchronizationClient)} connection failed");
            if (e.StatusCode == StatusCode.Unavailable)
            {
               await Task.Delay(100, cancellationToken);
            }
            else
            {
               return;
            }
         }
      }
   }

   public string Name => clientName;

   private string CreateClientName()
   {
      return Process.GetCurrentProcess().ProcessName;
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
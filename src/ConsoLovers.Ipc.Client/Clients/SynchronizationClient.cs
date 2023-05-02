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
public class SynchronizationClient : ISynchronizationClient, ISynchronizedClient
{
   private readonly IClientLogger logger;

   #region Constants and Fields

   private readonly SynchronizatioService.SynchronizatioServiceClient connectionService;

   private readonly int pollingDelay = 100;


   #endregion

   #region Constructors and Destructors

   public SynchronizationClient(GrpcChannel channel, IClientLogger logger, string socketPath)
   {
      if (channel == null)
         throw new ArgumentNullException(nameof(channel));
      SocketFilePath = socketPath ?? throw new ArgumentNullException(nameof(socketPath));

      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

      Id = Process.GetCurrentProcess().ProcessName;
      logger.Debug($"SynchronizationClient for process {Id} was created");
      connectionService = new SynchronizatioService.SynchronizatioServiceClient(channel);
      State = SyncState.NotConnected;
   }

   #endregion

   #region ISynchronizationClient Members

   public string SocketFilePath { get; }

   public string Id { get; }

   public void OnConnectionEstablished(CancellationToken cancellationToken)
   {
   }

   public void OnConnectionConfirmed(CancellationToken cancellationToken)
   {
   }

   public void OnConnectionAborted(CancellationToken cancellationToken)
   {
   }

   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      await SynchronizeAsync(cancellationToken, this);
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

   internal SyncState State { get; set; }

   public async Task SynchronizeAsync(CancellationToken cancellationToken, ISynchronizedClient client)
   {
      if (client == null)
         throw new ArgumentNullException(nameof(client));

      var handle = await EstablishConnectionAsync(client, cancellationToken);
      client.OnConnectionEstablished(cancellationToken);
      await ConfirmConnectionAsync(client, handle, cancellationToken);
   }

   private async Task ConfirmConnectionAsync(ISynchronizedClient client, string handle, CancellationToken cancellationToken)
   {
      logger.Debug($"{client.Id} tries to confirm the connection {handle}");
      var confirmRequest = new ConfirmConnectionRequest { Handle = handle };

      try
      {
         await connectionService.ConfirmConnectionAsync(confirmRequest, null, null, cancellationToken);
         logger.Debug($"{client.Id} confirmed connection {handle}");

         State = SyncState.ConnectionEstablished;
         client.OnConnectionConfirmed(cancellationToken);
      }
      catch (RpcException)
      {
         client.OnConnectionAborted(cancellationToken);
      }
   }

   private async Task<string> EstablishConnectionAsync(ISynchronizedClient client, CancellationToken cancellationToken)
   {
      logger.Debug($"{client.Id} tries to establish a connection");
      while (true)
      {
         cancellationToken.ThrowIfCancellationRequested();

         try
         {
            if (!SocketFileExists(client))
            {
               await Task.Delay(pollingDelay, cancellationToken);
               continue;
            }

            var connectionRequest = new EstablishConnectionRequest { ClientId = client.Id };
            var response = await connectionService.EstablishConnectionAsync(connectionRequest, null, null, cancellationToken);

            State = SyncState.Connecting;
            return response.Handle;
         }
         catch (RpcException e)
         {
            if (e.StatusCode != StatusCode.Unavailable)
               throw;

            logger.Trace($"{client.Id} failed to establish connection");
            await Task.Delay(pollingDelay, cancellationToken);
         }
      }

   }

   private bool SocketFileExists(ISynchronizedClient client)
   {
      return File.Exists(client.SocketFilePath);
   }

   #endregion
}

public enum SyncState
{
   NotConnected,
   Connecting,
   ConnectionEstablished
}
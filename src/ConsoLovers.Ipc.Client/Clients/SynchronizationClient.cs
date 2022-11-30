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

   private readonly string id;

   private readonly int pollingDelay = 100;

   private Task<AsyncDuplexStreamingCall<SynchronizeRequest, SynchronizeResponse>>? connectingTask;

   #endregion

   #region Constructors and Destructors

   public SynchronizationClient(GrpcChannel channel, IClientLogger logger)
   {
      if (channel == null)
         throw new ArgumentNullException(nameof(channel));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

      id = GenerateId();
      logger.Debug($"SynchronizationClient with name {id} was created");
      connectionService = new SynchronizatioService.SynchronizatioServiceClient(channel);
      State = SyncState.NotConnected;
   }

   #endregion

   #region ISynchronizationClient Members

   public string Id => id;

   private string GenerateId()
   {
      return $"{Process.GetCurrentProcess().ProcessName}-{Guid.NewGuid()}";
   }
   
   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      if (connectingTask == null)
      {
         // This means no real client was created yet, so we just try to establish a connection to the server
         await SynchronizeAsync(cancellationToken, _ => { });
      }
      else
      {
         // This means that a real client already initiated the connection
         // and we can wait for this task to finish here
         await connectingTask.WaitAsync(cancellationToken);
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

   internal SyncState State { get; set; }


   public async Task SynchronizeAsync(CancellationToken cancellationToken, Action<CancellationToken> onConnectionEstablished)
   {
      connectingTask = EstablishConnection(cancellationToken);
      var streamingCall = await connectingTask.WaitAsync(cancellationToken);

      onConnectionEstablished(cancellationToken);

      await CompleteSync(streamingCall, cancellationToken);
      await streamingCall.RequestStream.CompleteAsync();
   }

   private Task CompleteSync(AsyncDuplexStreamingCall<SynchronizeRequest, SynchronizeResponse> streamingCall, CancellationToken cancellationToken)
   {
      var request = new SynchronizeRequest
      {
         ClientId = Id,
         Action = SyncRequestAction.SynchronizationCompleted
      };

      return streamingCall.RequestStream.WriteAsync(request, cancellationToken);
   }

   private async Task<AsyncDuplexStreamingCall<SynchronizeRequest, SynchronizeResponse>> EstablishConnection(CancellationToken cancellationToken)
   {
      logger.Debug("SynchronizationClient is trying to establish a connection to server");
      while (State != SyncState.ConnectionEstablished)
      {
         cancellationToken.ThrowIfCancellationRequested();

         try
         {
            var streamingCall = connectionService.Synchronize();
            State = SyncState.Connecting;
            var connectRequest = new SynchronizeRequest { ClientId = Id, Action = SyncRequestAction.EstablishConnection };

            await streamingCall.RequestStream.WriteAsync(connectRequest, cancellationToken);
            logger.Debug($"{nameof(ISynchronizationClient)} could connect successfully");
            State = SyncState.ConnectionEstablished;
            return streamingCall;
         }
         catch (RpcException e)
         {
            logger.Trace($"{nameof(ISynchronizationClient)} connection failed");
            if (e.StatusCode == StatusCode.Unavailable)
            {
               await Task.Delay(pollingDelay, cancellationToken);
            }
            else
            {
               throw;
            }
         }
      }

      // TODO
      throw new InvalidOperationException();
   }


   #endregion
}

public enum SyncState
{
   NotConnected,
   Connecting,
   ConnectionEstablished
}
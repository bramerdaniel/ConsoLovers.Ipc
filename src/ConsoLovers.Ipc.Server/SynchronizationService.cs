// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Collections.Concurrent;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;
using global::Grpc.Net.Client;

/// <summary>
///    Build in gRPC service that allows the client to call a service function to check if the server is available. This is required as the
///    <see cref="GrpcChannel"/> functions did not work for unix domain sockets.
/// </summary>
/// <seealso cref="ConsoLovers.Ipc.Grpc.SynchronizatioService.SynchronizatioServiceBase"/>
internal class SynchronizationService : SynchronizatioService.SynchronizatioServiceBase
{
   private readonly ClientRegistry clientRegistry;

   private readonly IServerLogger logger;

   private readonly ConcurrentDictionary<string, string> handles;

   public SynchronizationService(ClientRegistry clientRegistry, IServerLogger logger)
   {
      this.clientRegistry = clientRegistry ?? throw new ArgumentNullException(nameof(clientRegistry));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      handles = new ConcurrentDictionary<string, string>();
   }

   public override Task<EstablishConnectionResponse> EstablishConnection(EstablishConnectionRequest request, ServerCallContext context)
   {
      var handle = Guid.NewGuid().ToString();
      if (handles.TryAdd(handle, request.ClientId))
      {
         logger.Debug($"{request.ClientId} established the connection {handle} ({Thread.CurrentThread.ManagedThreadId})");
         return Task.FromResult(new EstablishConnectionResponse { Handle = handle });
      }

      return Task.FromResult(new EstablishConnectionResponse { Handle = string.Empty });
   }

   public override Task<ConfirmConnectionResponse> ConfirmConnection(ConfirmConnectionRequest request, ServerCallContext context)
   {
      if (handles.TryRemove(request.Handle, out var client))
      {
         logger.Debug($"{client} confirmed the connection {request.Handle} ({Thread.CurrentThread.ManagedThreadId})");
         clientRegistry.NotifyClientConnected(client);
         return Task.FromResult(new ConfirmConnectionResponse());
      }

      throw new RpcException(new Status(StatusCode.NotFound, $"Handle {request.Handle} not found."));
   }

   #region Public Methods and Operators

   #endregion

}
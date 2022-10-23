﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;
using global::Grpc.Net.Client;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

/// <summary>
///    Build in gRPC service that allows the client to call a service function to check if the server is available. This is required as the
///    <see cref="GrpcChannel"/> functions did not work for unix domain sockets.
/// </summary>
/// <seealso cref="ConsoLovers.Ipc.Grpc.SynchronizatioService.SynchronizatioServiceBase"/>
internal class SynchronizationService : SynchronizatioService.SynchronizatioServiceBase
{



   private readonly IDiagnosticLogger logger;

   public SynchronizationService(IDiagnosticLogger logger )
   {
      
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   #region Public Methods and Operators

   public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
   {
      logger.Log("Client connected");
      return Task.FromResult(new ConnectResponse());
   }

   #endregion

}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

internal class ConnectionService : Grpc.ConnectionService.ConnectionServiceBase
{
   public ConnectionService()
   {
      
   }

   public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
   {
      return Task.FromResult(new ConnectResponse());
   }
}
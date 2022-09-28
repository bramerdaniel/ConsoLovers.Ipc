// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreeterService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IpcServerExtension.Service;

using global::Grpc.Core;

using IpcServerExtension.Grpc;

/// <summary>The hello world of gRPC</summary>
/// <seealso cref="IpcServerExtension.Grpc.GreeterService.GreeterServiceBase"/>
internal class GreeterService : Grpc.GreeterService.GreeterServiceBase
{
   #region Public Methods and Operators

   public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
   {
      return Task.FromResult(new HelloReply { Message = $"Hello from {request.Name}" });
   }

   #endregion
}
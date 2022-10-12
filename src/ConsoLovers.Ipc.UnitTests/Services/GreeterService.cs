// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Services;

using System.Threading.Tasks;

using ConsoLovers.Ipc.UnitTests.Grpc;

using global::Grpc.Core;

internal class GreeterService : Grpc.GreeterService.GreeterServiceBase
{
   public override Task<SayHelloResponse> SayHello(SayHelloRequest request, ServerCallContext context)
   {
      return Task.FromResult(new SayHelloResponse{Message = $"Hello {request.Name}" });
   }
}
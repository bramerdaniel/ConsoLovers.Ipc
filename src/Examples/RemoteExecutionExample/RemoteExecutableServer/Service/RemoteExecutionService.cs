// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Service;

using Grpc.Core;

using IpcServerExtension.Grpc;

/// <summary>The hello world of gRPC</summary>
/// <seealso cref="IpcServerExtension.Grpc.GreeterService.GreeterServiceBase"/>
internal class RemoteExecutionService : IpcServerExtension.Grpc.RemoteExecutionService.RemoteExecutionServiceBase
{
   #region Public Methods and Operators

   public override Task<ExecuteCommandResponse> ExecuteCommand(ExecuteCommandRequest request, ServerCallContext context)
   {



      return base.ExecuteCommand(request, context);
   }

   #endregion
}
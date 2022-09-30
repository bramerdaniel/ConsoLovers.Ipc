// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Service;

using Grpc.Core;

using IpcServerExtension.Grpc;

internal class RemoteExecutionService : IpcServerExtension.Grpc.RemoteExecutionService.RemoteExecutionServiceBase
{
   #region Constants and Fields

   private readonly IRemoteExecutionQueue executionQueue;

   #endregion

   #region Constructors and Destructors

   public RemoteExecutionService(IRemoteExecutionQueue executionQueue)
   {
      this.executionQueue = executionQueue;
   }

   #endregion

   #region Public Methods and Operators

   public override Task<ExecuteCommandResponse> ExecuteCommand(ExecuteCommandRequest request, ServerCallContext context)
   {
      executionQueue.Jobs.Writer.TryWrite(new RemoteJob(request.Name));
      return Task.FromResult(new ExecuteCommandResponse { Message = $"{request.Name} executed" });
   }

   #endregion
}
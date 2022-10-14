// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposeServerHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.Ipc;

internal class DisposeServerHandler : IAsyncShutdownHandler
{
   private readonly IIpcServer server;

   public DisposeServerHandler(IIpcServer server)
   {
      this.server = server;
   }

   public async Task NotifyShutdownAsync(IExecutionResult result)
   {
      await server.DisposeAsync();
   }
}
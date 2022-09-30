// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExecutingClient.Client;

using ConsoLovers.Ipc;

using IpcServerExtension.Grpc;

internal class RemoteExecutionClient : ConfigurableClient<RemoteExecutionService.RemoteExecutionServiceClient>, IRemoteExecutionClient
{
   public async Task<string> ExecuteCommandAsync(string name)
   {
      var commandRequest = new ExecuteCommandRequest { Name = name };
      var response = await ServiceClient.ExecuteCommandAsync(commandRequest);
      return response.Message;
   }
}
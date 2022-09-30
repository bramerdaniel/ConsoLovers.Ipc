// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteExecutionClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExecutingClient.Client;

using ConsoLovers.Ipc;

public interface IRemoteExecutionClient : IConfigurableClient
{
   Task<string> ExecuteCommandAsync(string name);
}
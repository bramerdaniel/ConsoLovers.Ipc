// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGreeterClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IpcServerExtension.Client;

using ConsoLovers.Ipc;

public interface IGreeterClient : IConfigurableClient
{
   Task<string> SayHelloAsync(string name);
}
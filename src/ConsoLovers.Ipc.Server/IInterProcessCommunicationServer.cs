// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterProcessCommunicationServer.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Server;

public interface IInterProcessCommunicationServer : IServiceProvider, IDisposable, IAsyncDisposable
{
}
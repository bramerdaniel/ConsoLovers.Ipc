// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIpcServerFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.Ipc;

public interface IIpcServerFactory
{
   IIpcServer Create();

   public string Server { get; set; }
}
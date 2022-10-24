// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIpcServerLifetime.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.Ipc;

public interface IIpcServerLifetime
{
   bool ServerCreated();

   bool GetServerWhenCreated(out IIpcServer server);
}
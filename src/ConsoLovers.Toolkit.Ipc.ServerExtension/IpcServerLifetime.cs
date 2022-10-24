// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerLifetime.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.Ipc;

internal class IpcServerLifetime : IIpcServerLifetime
{
   internal IIpcServer? Server { get; set; }

   public bool ServerCreated()
   {
      return Server != null;
   }

   public bool GetServerWhenCreated(out IIpcServer server)
   {
      server = Server!;
      return Server != null;
   }
}
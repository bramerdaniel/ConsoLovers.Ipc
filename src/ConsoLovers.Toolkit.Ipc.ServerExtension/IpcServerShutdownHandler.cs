// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerShutdownHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Toolkit.Ipc.ServerExtension;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

/// <summary><see cref="IAsyncShutdownHandler"/> that </summary>
/// <seealso cref="ConsoLovers.ConsoleToolkit.Core.IAsyncShutdownHandler"/>
internal class IpcServerShutdownHandler : IAsyncShutdownHandler
{
   #region Constants and Fields

   private readonly IIpcServer? server;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="IpcServerShutdownHandler"/> class.</summary>
   /// <param name="server">The server.</param>
   public IpcServerShutdownHandler(IIpcServer? server)
   {
      // TODO this injection would create the server if not happened yet
      // this should be done smarter
      this.server = server;
   }

   #endregion

   #region IAsyncShutdownHandler Members

   public async Task NotifyShutdownAsync(IExecutionResult result)
   {
      if (server == null)
         return;

      await server.DisposeAsync();
   }

   #endregion
}
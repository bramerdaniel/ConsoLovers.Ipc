// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerShutdownHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Toolkit.Ipc.ServerExtension;

using ConsoLovers.ConsoleToolkit.Core;

/// <summary><see cref="IAsyncShutdownHandler"/> that </summary>
/// <seealso cref="ConsoLovers.ConsoleToolkit.Core.IAsyncShutdownHandler"/>
internal class IpcServerShutdownHandler : IAsyncShutdownHandler
{
   #region Constants and Fields

   private readonly IpcServerLifetime serverLifetime;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="IpcServerShutdownHandler"/> class.</summary>
   /// <param name="serverLifetime">The server lifetime.</param>
   public IpcServerShutdownHandler(IpcServerLifetime serverLifetime)
   {
      this.serverLifetime = serverLifetime ?? throw new ArgumentNullException(nameof(serverLifetime));
   }

   #endregion

   #region IAsyncShutdownHandler Members

   public async Task NotifyShutdownAsync(IExecutionResult result)
   {
      if (serverLifetime.GetServerWhenCreated(out var server))
         await server.DisposeAsync();
   }

   #endregion
}
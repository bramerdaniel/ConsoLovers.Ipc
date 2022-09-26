// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServer.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Server;

/// <summary>Entry point for creating client or server classes for the inter process communication</summary>
public static class IpcServer
{
   #region Public Methods and Operators

   /// <summary>Creates a <see cref="IServerBuilder"/> for configuring the required <see cref="IInterProcessCommunicationServer"/>.</summary>
   /// <returns>A <see cref="IServerBuilderWithoutName"/></returns>
   public static IServerBuilderWithoutName CreateServer()
   {
      return new ServerBuilder();
   }

   #endregion
}
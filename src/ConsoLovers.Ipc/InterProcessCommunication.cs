// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterProcessCommunication.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Internals;

/// <summary>Entry point for creating client or server classes for the inter process communication</summary>
public static class InterProcessCommunication
{
   #region Public Methods and Operators

   /// <summary>Creates a <see cref="IClientFactoryBuilder"/> for configuring the required <see cref="IClientFactory"/>.</summary>
   /// <returns>A <see cref="IClientFactoryBuilder"/></returns>
   public static IClientFactoryBuilderWithoutName CreateClientFactory()
   {
      return new ClientFactoryBuilder();
   }

   /// <summary>Creates a <see cref="IServerBuilder"/> for configuring the required <see cref="IInterProcessCommunicationServer"/>.</summary>
   /// <returns>A <see cref="IServerBuilderWithoutName"/></returns>
   public static IServerBuilderWithoutName CreateServer()
   {
      return new ServerBuilder();
   }

   #endregion
}
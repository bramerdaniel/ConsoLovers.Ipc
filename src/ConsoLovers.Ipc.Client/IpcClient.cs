// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Entry point for the client side of the inter-process communication setup</summary>
public static class IpcClient
{
   /// <summary>Creates a <see cref="IClientFactoryBuilder"/> for configuring the required <see cref="IClientFactory"/>.</summary>
   /// <returns>A <see cref="IClientFactoryBuilder"/></returns>
   public static IClientFactoryBuilderWithoutName CreateClientFactory()
   {
      return new ClientFactoryBuilder();
   }

}
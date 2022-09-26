// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Client;

using ConsoLovers.Ipc.Internals;

public static class IpcClient
{

   /// <summary>Creates a <see cref="IClientFactoryBuilder"/> for configuring the required <see cref="IClientFactory"/>.</summary>
   /// <returns>A <see cref="IClientFactoryBuilder"/></returns>
   public static IClientFactoryBuilderWithoutName CreateClientFactory()
   {
      return new ClientFactoryBuilder();
   }

}
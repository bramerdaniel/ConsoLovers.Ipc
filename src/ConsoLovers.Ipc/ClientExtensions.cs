// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Result;
using ConsoLovers.Ipc.Services;

using Microsoft.Extensions.DependencyInjection;

public static class ClientExtensions
{
   #region Public Methods and Operators

   public static IClientFactoryBuilder AddProgressClient(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      clientFactoryBuilder.AddService(s => s.AddSingleton<IProgressClient, ProgressClient>());
      return clientFactoryBuilder;
   }

   public static IClientFactoryBuilder AddResultClient(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      clientFactoryBuilder.AddService(s => s.AddSingleton<IResultClient, ResultClient>());
      return clientFactoryBuilder;
   }

   #endregion
}
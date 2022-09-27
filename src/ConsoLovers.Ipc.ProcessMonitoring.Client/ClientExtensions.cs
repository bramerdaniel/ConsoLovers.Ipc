// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.ProcessMonitoring;

using Microsoft.Extensions.DependencyInjection;

public static class ClientExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds the <see cref="ICancellationClient"/>.</summary>
   /// <param name="clientFactoryBuilder">The client factory builder.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> for more fluent setup</returns>
   /// <exception cref="System.ArgumentNullException">clientFactoryBuilder</exception>
   public static IClientFactoryBuilder AddCancellationClient(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      clientFactoryBuilder.AddService(s => s.AddSingleton<ICancellationClient, CancellationClient>());
      return clientFactoryBuilder;
   }

   /// <summary>Adds the default services that are build in .</summary>
   /// <param name="clientFactoryBuilder">The client factory builder.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> for more fluent setup</returns>
   public static IClientFactoryBuilder AddProcessMonitoringClients(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      return clientFactoryBuilder
         .AddProgressClient()
         .AddResultClient()
         .AddCancellationClient();
   }

   /// <summary>Adds the <see cref="IProgressClient"/>.</summary>
   /// <param name="clientFactoryBuilder">The client factory builder.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> for more fluent setup</returns>
   /// <exception cref="System.ArgumentNullException">clientFactoryBuilder</exception>
   public static IClientFactoryBuilder AddProgressClient(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      clientFactoryBuilder.AddService(s => s.AddSingleton<IProgressClient, ProgressClient>());
      return clientFactoryBuilder;
   }
   

   /// <summary>Adds the <see cref="IResultClient"/>.</summary>
   /// <param name="clientFactoryBuilder">The client factory builder.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> for more fluent setup</returns>
   /// <exception cref="System.ArgumentNullException">clientFactoryBuilder</exception>
   public static IClientFactoryBuilder AddResultClient(this IClientFactoryBuilder clientFactoryBuilder)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));

      clientFactoryBuilder.AddService(s => s.AddSingleton<IResultClient, ResultClient>());
      return clientFactoryBuilder;
   }

   #endregion
}
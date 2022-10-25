// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

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

      clientFactoryBuilder.AddService(s => s.AddTransient<IProgressClient, ProgressClient>());
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

      clientFactoryBuilder.AddService(s => s.AddTransient<IResultClient, ResultClient>());
      return clientFactoryBuilder;
   }

   /// <summary>Waits for the result to be available.</summary>
   /// <param name="resultClient">The result client.</param>
   /// <returns>The <see cref="ResultInfo"/></returns>
   public static Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient)
   {
      return resultClient.WaitForResultAsync(CancellationToken.None);
   }

   /// <summary>Waits for the result to be available.</summary>
   /// <param name="resultClient">The result client.</param>
   /// <param name="timeout">The timeout after the waiting should be canceled.</param>
   /// <returns>The <see cref="ResultInfo"/></returns>
   public static Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient, TimeSpan timeout)
   {
      var tokenSource = new CancellationTokenSource();
      tokenSource.CancelAfter(timeout);
      return resultClient.WaitForResultAsync(tokenSource.Token);
   }

   /// <summary>Waits for the result to be available.</summary>
   /// <param name="resultClient">The result client.</param>
   /// <param name="timeoutInMilliseconds">The timeout in milliseconds after the waiting should be canceled.</param>
   /// <returns>The <see cref="ResultInfo"/></returns>
   public static Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient, int timeoutInMilliseconds)
   {
      return resultClient.WaitForResultAsync(TimeSpan.FromMilliseconds(timeoutInMilliseconds));
   }

   #endregion
}
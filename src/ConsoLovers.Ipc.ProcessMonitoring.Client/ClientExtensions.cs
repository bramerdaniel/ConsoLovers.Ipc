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

   public static async Task WaitForCompletedAsync(this IProgressClient progressClient)
   {
      if (progressClient == null)
         throw new ArgumentNullException(nameof(progressClient));
      
      await progressClient.WaitForCompletedAsync(CancellationToken.None);
   }

   public static async Task WaitForCompletedAsync(this IProgressClient progressClient, int timeoutInMilliseconds)
   {
      if (progressClient == null)
         throw new ArgumentNullException(nameof(progressClient));
      
      var tokenSource = new CancellationTokenSource(timeoutInMilliseconds);
      await progressClient.WaitForCompletedAsync(tokenSource.Token);
   }

   public static async Task WaitForCompletedAsync(this IProgressClient progressClient, TimeSpan timeout)
   {
      if (progressClient == null)
         throw new ArgumentNullException(nameof(progressClient));
      
      var tokenSource = new CancellationTokenSource(timeout);
      await progressClient.WaitForCompletedAsync(tokenSource.Token);
   }


   public static async Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient)
   {
      if (resultClient == null)
         throw new ArgumentNullException(nameof(resultClient));
      
      return await resultClient.WaitForResultAsync(CancellationToken.None);
   }

   public static async Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient, int timeoutInMilliseconds)
   {
      if (resultClient == null)
         throw new ArgumentNullException(nameof(resultClient));

      var tokenSource = new CancellationTokenSource(timeoutInMilliseconds);
      return await resultClient.WaitForResultAsync(tokenSource.Token);
   }

   public static async Task<ResultInfo> WaitForResultAsync(this IResultClient resultClient, TimeSpan timeout)
   {
      if (resultClient == null)
         throw new ArgumentNullException(nameof(resultClient));

      var tokenSource = new CancellationTokenSource(timeout);
      return await resultClient.WaitForResultAsync(tokenSource.Token);
   }


   #endregion
}
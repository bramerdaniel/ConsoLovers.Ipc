// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.ProcessMonitoring;

/// <summary>Extension methods for the <see cref="IClientFactory"/> interface</summary>
public static class ClientFactoryExtensions
{
   #region Public Methods and Operators

   /// <summary>Gets the <see cref="ICancellationClient"/> from the <see cref="IClientFactory"/>.</summary>
   /// <param name="clientFactory">The client factory.</param>
   /// <returns>The created <see cref="ICancellationClient"/></returns>
   /// <exception cref="System.ArgumentNullException">clientFactory</exception>
   public static ICancellationClient CreateCancellationClient(this IClientFactory clientFactory)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.CreateClient<ICancellationClient>();
   }

   /// <summary>Gets the <see cref="IProgressClient"/> from the <see cref="IClientFactory"/>.</summary>
   /// <param name="clientFactory">The client factory.</param>
   /// <returns>The created <see cref="IProgressClient"/></returns>
   /// <exception cref="System.ArgumentNullException">clientFactory</exception>
   public static IProgressClient CreateProgressClient(this IClientFactory clientFactory)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.CreateClient<IProgressClient>();
   }

   /// <summary>Gets the <see cref="IResultClient"/> from the <see cref="IClientFactory"/>.</summary>
   /// <param name="clientFactory">The client factory.</param>
   /// <returns>The created <see cref="IResultClient"/></returns>
   /// <exception cref="System.ArgumentNullException">clientFactory</exception>
   public static IResultClient CreateResultClient(this IClientFactory clientFactory)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.CreateClient<IResultClient>();
   }

   public static Task WaitForServerAsync(this IClientFactory clientFactory, CancellationToken cancellationToken)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.SynchronizationClient.WaitForServerAsync(cancellationToken);
   }

   public static Task WaitForServerAsync(this IClientFactory clientFactory, TimeSpan timeout)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.SynchronizationClient.WaitForServerAsync(timeout);
   }

   public static Task WaitForServerAsync(this IClientFactory clientFactory, TimeSpan timeout, CancellationToken cancellationToken)
   {
      if (clientFactory == null)
         throw new ArgumentNullException(nameof(clientFactory));

      return clientFactory.SynchronizationClient.WaitForServerAsync(timeout, cancellationToken);
   }

   #endregion
}
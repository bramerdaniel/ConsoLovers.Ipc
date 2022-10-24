// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactory.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;
using System.Text;

using Microsoft.Extensions.DependencyInjection;

internal class ClientFactory : IClientFactory
{
   #region Constructors and Destructors

   internal ClientFactory(IServiceProvider serviceProvider, IChannelFactory channelFactory, CultureInfo? culture)
   {
      ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      ChannelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
      Culture = culture;
   }

   #endregion

   #region IClientFactory Members

   /// <summary>Gets the channel factory the <see cref="IClientFactory"/> uses.</summary>
   public IChannelFactory ChannelFactory { get; }

   /// <summary>Gets the <see cref="ISynchronizationClient"/>.</summary>
   public ISynchronizationClient SynchronizationClient => ChannelFactory.SynchronizationClient;

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <returns>The created client</returns>
   public T CreateClient<T>()
      where T : class, IConfigurableClient
   {
      return CreateClient<T>(Culture);
   }

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <param name="culture">The culture the client will be running in.</param>
   /// <returns>The created client</returns>
   public T CreateClient<T>(CultureInfo? culture)
      where T : class, IConfigurableClient
   {
      var client = ServiceProvider.GetService<T>() ?? CreateInstance<T>();
      var configuration = new ClientConfiguration(ChannelFactory, culture ?? Culture);
      client.Configure(configuration);
      
      return client;
   }

   public T CreateClient<T>(string culture)
      where T : class, IConfigurableClient
   {
      if (culture == null)
         throw new ArgumentNullException(nameof(culture));

      var cultureInfo = CultureInfo.GetCultureInfo(culture);
      return CreateClient<T>(cultureInfo);
   }

   #endregion

   #region Properties

   /// <summary>Gets the default culture the factory will use to create clients.</summary>
   private CultureInfo? Culture { get; }

   /// <summary>Gets the service provider.</summary>
   private IServiceProvider ServiceProvider { get; }

   #endregion

   #region Methods

   private static void CheckForBuildInClients(Type serviceType)
   {
      if (serviceType.Name == "IResultClient")
         throw new InvalidOperationException(CreateMessage(serviceType, "AddResultClient"));
      if (serviceType.Name == "IProgressClient")
         throw new InvalidOperationException(CreateMessage(serviceType, "AddProgressClient"));
      if (serviceType.Name == "ICancellationClient")
         throw new InvalidOperationException(CreateMessage(serviceType, "AddCancellationClient"));
   }

   private static string CreateMessage(Type serviceType, string addMethod)
   {
      var builder = new StringBuilder();
      builder.AppendLine($"The client {serviceType.Name} was not registered at the {nameof(IClientFactory)}.");
      builder.AppendLine($"Call {addMethod}() on the factory builder to add the requested client.");
      return builder.ToString();
   }

   private T CreateInstance<T>()
      where T : class, IConfigurableClient
   {
      CheckForBuildInClients(typeof(T));
      return ActivatorUtilities.CreateInstance<T>(ServiceProvider);
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Internals;

using System.Text;

using Microsoft.Extensions.DependencyInjection;

internal class ClientFactory : IClientFactory
{
   #region Constructors and Destructors

   internal ClientFactory(IServiceProvider serviceProvider, IChannelFactory channelFactory)
   {
      ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      ChannelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
   }

   #endregion

   #region IClientFactory Members

   /// <summary>Gets the channel factory the <see cref="IClientFactory"/> uses.</summary>
   public IChannelFactory ChannelFactory { get; }

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <returns></returns>
   public T CreateClient<T>()
      where T : class, IConfigurableClient
   {
      var client = ServiceProvider.GetService<T>() ?? CreateInstance<T>();
      client.Configure(new ClientConfiguration(ChannelFactory));
      return client;
   }

   private T CreateInstance<T>()
      where T : class, IConfigurableClient
   {
      CheckForBuildInClients(typeof(T));

      return ActivatorUtilities.CreateInstance<T>(ServiceProvider);
   }

   private static void CheckForBuildInClients(Type serviceType)
   {
      if (serviceType == typeof(IResultClient))
         throw new InvalidOperationException(CreateMessage(serviceType, nameof(ClientExtensions.AddResultClient)));
      if (serviceType == typeof(IProgressClient))
         throw new InvalidOperationException(CreateMessage(serviceType, nameof(ClientExtensions.AddProgressClient)));
      if (serviceType == typeof(ICancellationClient))
         throw new InvalidOperationException(CreateMessage(serviceType, nameof(ClientExtensions.AddCancellationClient)));
   }

   private static string CreateMessage(Type serviceType, string addMethod)
   {
      var builder = new StringBuilder();
      builder.AppendLine($"The client {serviceType.Name} was not registered at the {nameof(IClientFactory)}.");
      builder.AppendLine($"Call {addMethod}() on the factory builder to add the requested client.");
      return builder.ToString();
   }

   #endregion

   #region Properties

   private IServiceProvider ServiceProvider { get; }

   #endregion
}
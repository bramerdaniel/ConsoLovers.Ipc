// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Internals;

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
      var client = ServiceProvider.GetService<T>() ?? ActivatorUtilities.CreateInstance<T>(ServiceProvider);
      client.Configure(new ClientConfiguration(ChannelFactory));
      return client;
   }

   #endregion

   #region Properties

   private IServiceProvider ServiceProvider { get; }

   #endregion
}
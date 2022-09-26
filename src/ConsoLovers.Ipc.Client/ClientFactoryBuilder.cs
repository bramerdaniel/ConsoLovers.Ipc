// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

using Microsoft.Extensions.DependencyInjection;

internal class ClientFactoryBuilder : IClientFactoryBuilder, IClientFactoryBuilderWithoutName
{
   #region Constants and Fields

   private readonly ServiceCollection serviceCollection;

   private ChannelFactory? channelFactory;

   #endregion

   #region Constructors and Destructors

   public ClientFactoryBuilder()
   {
      serviceCollection = new ServiceCollection();
      serviceCollection.AddSingleton<IClientFactoryBuilder>(this);
   }

   #endregion

   #region IClientFactoryBuilder Members

   public IClientFactoryBuilder AddService(Action<ServiceCollection> services)
   {
      if (services == null)
         throw new ArgumentNullException(nameof(services));

      services(serviceCollection);
      return this;
   }

   public IClientFactory Build()
   {
      if (channelFactory == null)
         throw new InvalidOperationException($"The {nameof(channelFactory)} is not specified yet");

      var providerFactory = new DefaultServiceProviderFactory(new ServiceProviderOptions { ValidateOnBuild = true });
      var serviceProvider = providerFactory.CreateServiceProvider(serviceCollection);

      return new ClientFactory(serviceProvider, channelFactory);
   }

   #endregion

   #region IClientFactoryBuilderWithoutName Members

   public IClientFactoryBuilder ForName(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      return InitializeWithName(name);
   }

   public IClientFactoryBuilder ForProcess(Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));

      return ForName(process.GetServerName());
   }

   #endregion

   #region Methods

   private IClientFactoryBuilder InitializeWithName(string name)
   {
      Validation.EnsureValidFileName(name);

      channelFactory = new ChannelFactory(name);
      serviceCollection.AddSingleton(channelFactory);
      return this;
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientFactoryBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Builder that can create a <see cref="IClientFactory"/></summary>
public interface IClientFactoryBuilder
{
   #region Public Methods and Operators

   /// <summary>Adds a service as client.</summary>
   /// <typeparam name="T">The client type</typeparam>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder AddClient<T>()
      where T : class, IConfigurableClient;

   /// <summary>Adds a service to the <see cref="IClientFactoryBuilder"/>.</summary>
   /// <param name="services">The services.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder AddService(Action<ServiceCollection> services);

   /// <summary>Builds the <see cref="IClientFactory"/>.</summary>
   /// <returns>The created <see cref="IClientFactory"/></returns>
   IClientFactory Build();

   /// <summary>Specifies the default culture the clients will be created with.</summary>
   /// <param name="culture">
   ///    The default client culture every client will be created with, when no other culture is specified in the
   ///    <see cref="IClientFactory.CreateClient{T}()"/> method.
   /// </param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder WithDefaultCulture(CultureInfo culture);

   /// <summary>Specifies the default culture the clients will be created with.</summary>
   /// <param name="culture">
   ///    The default client culture name every client will be created with, when no other culture is specified in the
   ///    <see cref="IClientFactory.CreateClient{T}()"/> method.
   /// </param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder WithDefaultCulture(string culture);

   /// <summary>Specifies the <see cref="IClientLogger"/> that should be used.</summary>
   /// <param name="logger">The logger to use.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder WithLogger(IClientLogger logger);

   #endregion
}
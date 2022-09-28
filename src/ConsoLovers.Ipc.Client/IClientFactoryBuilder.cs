// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientFactoryBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Builder that can create a <see cref="IClientFactory"/></summary>
public interface IClientFactoryBuilder
{
   #region Public Methods and Operators

   /// <summary>Adds a service to the <see cref="IClientFactoryBuilder"/>.</summary>
   /// <param name="services">The services.</param>
   /// <returns>The <see cref="IClientFactoryBuilder"/> the method was called on</returns>
   IClientFactoryBuilder AddService(Action<ServiceCollection> services);

   /// <summary>Builds the <see cref="IClientFactory"/>.</summary>
   /// <returns>The created <see cref="IClientFactory"/></returns>
   IClientFactory Build();

   #endregion
}
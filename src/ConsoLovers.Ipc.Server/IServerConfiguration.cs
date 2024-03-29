﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerConfiguration.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Part of the <see cref="IServerBuilder"/> interface, without the Start method</summary>
public interface IServerConfiguration
{
   #region Public Methods and Operators

   /// <summary>Adds the specified service as gRPC service to the server.</summary>
   /// <typeparam name="T">The gRPC service type</typeparam>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder AddGrpcService<T>()
      where T : class;

   /// <summary>Adds the specified service as gRPC service to the server.</summary>
   /// <typeparam name="T">The gRPC service type</typeparam>
   /// <param name="lifetime">The lifetime.</param>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder AddGrpcService<T>(ServiceLifetime lifetime)
      where T : class;

   /// <summary>Adds the specified service as gRPC service to the server.</summary>
   /// <param name="serviceType">Type of the service.</param>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder AddGrpcService(Type serviceType);

   /// <summary>Adds the service to the internal <see cref="IServiceCollection"/>.</summary>
   /// <param name="serviceSetup">The service setup.</param>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder AddService(Action<IServiceCollection> serviceSetup);

   /// <summary>Configures one or more services after dependency injection is ready.</summary>
   /// <param name="serviceConfig">The service configuration action.</param>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder ConfigureService(Action<IServiceProvider> serviceConfig);

   /// <summary>Configures the specified service of type T after dependency injection is ready.</summary>
   /// <param name="serviceConfig">The service configuration action.</param>
   /// <returns>The server builder for more fluent configuration</returns>
   IServerBuilder ConfigureService<T>(Action<T> serviceConfig)
      where T : class;

   #endregion
}
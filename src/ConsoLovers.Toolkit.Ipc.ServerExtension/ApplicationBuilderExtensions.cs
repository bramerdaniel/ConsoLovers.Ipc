// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>A bunch of extensions methods for the IApplicationBuilder{T} interface</summary>
public static class ApplicationBuilderExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds the specified <see cref="serviceType"/> as gPRC service to the <see cref="IIpcServer"/>.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The builder.</param>
   /// <param name="serviceType">Type of the gRPC service.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   public static IApplicationBuilder<T> AddGrpcService<T>(this IApplicationBuilder<T> builder, Type serviceType)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.ConfigureService(x =>
      {
         var serverBuilder = x.GetRequiredService<IServerBuilder>();
         serverBuilder.AddGrpcService(serviceType);
      });

      return builder;
   }

   /// <summary>Adds an <see cref="IIpcServer"/> to the applications services and uses the current process for computing the address.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The builder.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IApplicationBuilder<T> AddIpcServer<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.AddIpcServer(b => b.ForCurrentProcess());
   }

   /// <summary>Adds an <see cref="IIpcServer"/> to the applications services.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The builder.</param>
   /// <param name="name">The name.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IApplicationBuilder<T> AddIpcServer<T>(this IApplicationBuilder<T> builder, string name)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.AddIpcServer(b => b.ForName(name));
   }

   /// <summary>Adds an <see cref="IIpcServer"/> to the applications services.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The builder.</param>
   /// <param name="config">The configuration.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IApplicationBuilder<T> AddIpcServer<T>(this IApplicationBuilder<T> builder, Action<IServerBuilderWithoutName> config)
      where T : class
   {
      return builder.AddIpcServer(config, true);
   }

   /// <summary>Adds an <see cref="IIpcServer"/> to the applications services.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The builder.</param>
   /// <param name="config">The configuration action.</param>
   /// <param name="removeAspNetCoreLogging">if set to <c>true</c> the loggers of ASPNetCore will be removed, otherwise they will log to the console.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IApplicationBuilder<T> AddIpcServer<T>(this IApplicationBuilder<T> builder, Action<IServerBuilderWithoutName> config,
      bool removeAspNetCoreLogging)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));
      if (config == null)
         throw new ArgumentNullException(nameof(config));

      // TODO ensure there is only one IpcServer !
      builder.AddService(x => x.AddSingleton(_ => CreateServerBuilder(config, removeAspNetCoreLogging)));
      builder.AddService(x => x.AddSingleton(CreateIpcServer));
      return builder;
   }

   /// <summary>Configures the <see cref="IIpcServer"/> with the passed <see cref="IServerBuilder"/>.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The <see cref="IServerConfiguration"/> of the <see cref="IIpcServer"/>.</param>
   /// <param name="configAction">The configuration action.</param>
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configAction</exception>
   public static IApplicationBuilder<T> ConfigureIpcServer<T>(this IApplicationBuilder<T> builder, Action<IServerConfiguration> configAction)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));
      if (configAction == null)
         throw new ArgumentNullException(nameof(configAction));

      return builder.ConfigureService(ConfigurationAction);

      void ConfigurationAction(IServiceProvider serviceProvider)
      {
         configAction(serviceProvider.GetRequiredService<IServerBuilder>());
      }
   }

   #endregion

   #region Methods

   private static IIpcServer CreateIpcServer(IServiceProvider services)
   {
      var builder = services.GetRequiredService<IServerBuilder>();
      return builder.Start();
   }

   private static IServerBuilder CreateServerBuilder(Action<IServerBuilderWithoutName> config, bool removeAspLoggin)
   {
      var builderWithoutName = IpcServer.CreateServer();
      config(builderWithoutName);
      var serverBuilder = (IServerBuilder)builderWithoutName;
      if (removeAspLoggin)
         serverBuilder.RemoveAspNetCoreLogging();

      return serverBuilder;
   }

   #endregion
}
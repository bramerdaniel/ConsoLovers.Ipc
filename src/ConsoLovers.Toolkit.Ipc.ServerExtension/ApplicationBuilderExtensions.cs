// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

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
      builder.ConfigureService(x =>
      {
         var serverBuilder = x.GetRequiredService<IServerBuilder>();
         serverBuilder.AddGrpcService(serviceType);
      });

      return builder;
   }

   /// <summary>Adds an <see cref="IIpcServer"/> to the applications services.</summary>
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
   /// <returns>The current <see cref="IApplicationBuilder{T}"/> for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IApplicationBuilder<T> AddIpcServer<T>(this IApplicationBuilder<T> builder, Func<IServerBuilderWithoutName, IServerBuilder> config)
      where T : class
   {
      if (config == null)
         throw new ArgumentNullException(nameof(config));

      builder.AddService(x => x.AddSingleton(_ => CreateServerBuilder(config)));
      builder.AddService(x => x.AddSingleton(CreateIpcServer));
      return builder;
   }

   /// <summary>Configures the <see cref="IIpcServer"/> with the passed <see cref="IServerBuilder"/>.</summary>
   /// <typeparam name="T">The argument type of the application</typeparam>
   /// <param name="builder">The <see cref="IServerConfiguration"/> of the <see cref="IIpcServer"/>.</param>
   /// <param name="configAction">The configuration action.</param>
   /// <returns></returns>
   /// <exception cref="System.ArgumentNullException">configAction</exception>
   public static IApplicationBuilder<T> ConfigureIpcServer<T>(this IApplicationBuilder<T> builder, Action<IServerConfiguration> configAction)
      where T : class
   {
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

   private static IServerBuilder CreateServerBuilder(Func<IServerBuilderWithoutName, IServerBuilder> config)
   {
      var builderWithoutName = IpcServer.CreateServer();
      var builder = config(builderWithoutName);

      return builder;
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;

using LoggingExtensions::Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class ServerExtensions
{
   #region Public Methods and Operators

   /// <summary>Configures the web application before it is build.</summary>
   /// <param name="builder">The builder.</param>
   /// <param name="configureWebApplication">The configure web application.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configureWebApplication</exception>
   /// <exception cref="System.ArgumentException">The specified {nameof(builder)} must be of type {{typeof(ServerBuilder).Name}}</exception>
   public static IServerBuilder ConfigureWebApplication(this IServerBuilder builder, Action<WebApplicationBuilder> configureWebApplication)
   {
      if (configureWebApplication == null)
         throw new ArgumentNullException(nameof(configureWebApplication));

      if (builder is not ServerBuilder serverBuilder)
         throw new ArgumentException($"The specified {nameof(builder)} must be of type {{typeof(ServerBuilder).Name}}");

      configureWebApplication(serverBuilder.WebApplicationBuilder);
      return builder;
   }

   /// <summary>Adds gRPC reflection to the server services.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   public static IServerBuilder AddGrpcReflection(this IServerBuilder builder)
   {
      return builder.ConfigureWebApplication(app => app.Services.AddGrpcReflection());
   }

   /// <summary>Adds a service as singleton only if it was not already added.</summary>
   /// <typeparam name="TService">The type of the service.</typeparam>
   /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
   /// <param name="serviceCollection">The service collection.</param>
   /// <returns>True if the service was added</returns>
   public static bool EnsureSingleton<TService, TImplementation>(this IServiceCollection serviceCollection)
      where TImplementation : TService where TService : class
   {
      if (TryAddSingleton<TImplementation>(serviceCollection))
      {
         serviceCollection.AddSingleton<TService>(x => x.GetRequiredService<TImplementation>());
         return true;
      }

      return false;
   }

   /// <summary>Gets a <see cref="IServerBuilder"/> for the current process.</summary>
   /// <param name="builder">The builder without the server name specified.</param>
   /// <returns>The <see cref="IServerBuilder"/></returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder ForCurrentProcess(this IServerBuilderWithoutName builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.ForProcess(Process.GetCurrentProcess());
   }

   /// <summary>Configures the web application before it is build.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configureWebApplication</exception>
   /// <exception cref="System.ArgumentException">The specified {nameof(builder)} must be of type {{typeof(ServerBuilder).Name}}</exception>
   public static IServerBuilder RemoveAspNetCoreLogging(this IServerBuilder builder)
   {
      builder.ConfigureWebApplication(webApp => { webApp.Logging.ClearProviders(); });

      return builder;
   }

   #endregion

   #region Methods

   private static bool TryAddSingleton<T>(IServiceCollection serviceCollection)
   {
      var implementationType = typeof(T);
      if (serviceCollection.Any(x => x.ServiceType == implementationType))
         return false;

      serviceCollection.AddSingleton(implementationType);
      return true;
   }

   #endregion
}
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
   /// <returns></returns>
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

   /// <summary>Gets the <see cref="ICancellationHandler"/> that notifies the server to cancel.</summary>
   /// <param name="server">The server.</param>
   /// <returns>The <see cref="ICancellationHandler"/> service</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static ICancellationHandler GetCancellationHandler(this IInterProcessCommunicationServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<ICancellationHandler>();
   }

   /// <summary>Gets the <see cref="IProgressReporter"/> service.</summary>
   /// <param name="server">The <see cref="IInterProcessCommunicationServer"/> that provided the service.</param>
   /// <returns>The <see cref="IProgressReporter"/> to use</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IProgressReporter GetProgressReporter(this IInterProcessCommunicationServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<IProgressReporter>();
   }

   /// <summary>Gets the result reporter service.</summary>
   /// <param name="server">The server.</param>
   /// <returns>The result reporter service</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IResultReporter GetResultReporter(this IInterProcessCommunicationServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<IResultReporter>();
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
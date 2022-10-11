// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;
using System.Globalization;

using global::Grpc.Core;

using LoggingExtensions::Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

public static class ServerExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds gRPC reflection to the server services.</summary>
   /// <param name="builder">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   public static T AddGrpcReflection<T>(this T builder)
      where T : IServerConfiguration
   {
      return builder.ConfigureWebApplication(app => app.Services.AddGrpcReflection());
   }

   /// <summary>Configures the web application before it is build.</summary>
   /// <param name="builder">The configuration.</param>
   /// <param name="configureWebApplication">The configure web application.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configureWebApplication</exception>
   /// <exception cref="System.ArgumentException">The specified {nameof(configuration)} must be of type {{typeof(ServerBuilder).Name}}</exception>
   public static T ConfigureWebApplication<T>(this T builder, Action<WebApplicationBuilder> configureWebApplication)
      where T : IServerConfiguration
   {
      if (configureWebApplication == null)
         throw new ArgumentNullException(nameof(configureWebApplication));

      if (builder is not ServerBuilder serverBuilder)
         throw new ArgumentException($"The specified {nameof(builder)} must be of type {{typeof(ServerBuilder).Name}}");

      configureWebApplication(serverBuilder.WebApplicationBuilder);
      return builder;
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
   /// <param name="builder">The configuration without the server name specified.</param>
   /// <returns>The <see cref="IServerBuilder"/></returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static IServerBuilder ForCurrentProcess(this IServerBuilderWithoutName builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.ForProcess(Process.GetCurrentProcess());
   }

   /// <summary>Gets the <see cref="CultureInfo"/> the client requested.</summary>
   /// <param name="context">The <see cref="ServerCallContext"/>.</param>
   /// <returns>The requested culture</returns>
   public static CultureInfo GetCulture(this ServerCallContext context)
   {
      var languageHeader = context.RequestHeaders.Get(HeaderNames.AcceptLanguage);
      if (languageHeader != null)
      {
         var culture = languageHeader.Value;
         if (!string.IsNullOrWhiteSpace(culture))
         {
            try
            {
               return CultureInfo.GetCultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
               // we ignore unknown cultures here
            }
         }
      }

      return CultureInfo.InvariantCulture;
   }

   /// <summary>Configures the web application before it is build.</summary>
   /// <param name="configuration">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configureWebApplication</exception>
   /// <exception cref="System.ArgumentException">The specified {nameof(configuration)} must be of type {{typeof(ServerBuilder).Name}}</exception>
   public static T RemoveAspNetCoreLogging<T>(this T configuration)
      where T : IServerConfiguration
   {
      configuration.ConfigureWebApplication(webApp => { webApp.Logging.ClearProviders(); });
      return configuration;
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessMonitoringConfigurationExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.ProcessMonitoring.Cancellation;
using ConsoLovers.Ipc.ProcessMonitoring.Result;
using ConsoLovers.Ipc.ProcessMonitoring.Services;

using Microsoft.Extensions.DependencyInjection;

/// <summary>A bunch of extensions for the <see cref="IServerConfiguration"/> interface</summary>
public static class ProcessMonitoringConfigurationExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds the services required for cancellation.</summary>
   /// <param name="builder">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static T AddCancellationHandler<T>(this T builder)
      where T : IServerConfiguration
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(AddRequiredServices);

      void AddRequiredServices(IServiceCollection serviceCollection)
      {
         if (serviceCollection.EnsureSingleton<ICancellationHandler, CancellationHandler>())
            builder.AddGrpcService<CancellationService>();
      }

      return builder;
   }

   /// <summary>Adds the services required for cancellation.</summary>
   /// <param name="configuration">The configuration.</param>
   /// <param name="cancellationAction">The cancellation action.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static T AddCancellationHandler<T>(this T configuration, Func<bool> cancellationAction)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));
      if (cancellationAction == null)
         throw new ArgumentNullException(nameof(cancellationAction));

      configuration.AddCancellationHandler();
      configuration.ConfigureService<ICancellationHandler>(x => x.OnCancellationRequested(cancellationAction));

      return configuration;
   }

   /// <summary>Adds all services required for the process monitoring.</summary>
   /// <param name="configuration">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static T AddProcessMonitoring<T>(this T configuration)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));

      return configuration.RemoveAspNetCoreLogging()
         .AddProgressReporter()
         .AddResultReporter()
         .AddCancellationHandler();
   }

   /// <summary>Adds the services required for reporting progress to a client.</summary>
   /// <param name="configuration">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static T AddProgressReporter<T>(this T configuration)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));

      configuration.AddService(AddRequiredServices);

      void AddRequiredServices(IServiceCollection serviceCollection)
      {
         if (serviceCollection.EnsureSingleton<IProgressReporter, ProgressReporter>())
            configuration.AddGrpcService<ProgressService>();
      }

      return configuration;
   }

   /// <summary>Adds the services required for reporting the detailed execution result to a client.</summary>
   /// <param name="configuration">The configuration.</param>
   /// <returns>The configuration for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration</exception>
   public static T AddResultReporter<T>(this T configuration)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));

      configuration.AddService(AddRequiredServices);

      void AddRequiredServices(IServiceCollection serviceCollection)
      {
         if (serviceCollection.EnsureSingleton<IResultReporter, ResultReporter>())
            configuration.AddGrpcService<ResultService>();
      }

      return configuration;
   }

   #endregion
}
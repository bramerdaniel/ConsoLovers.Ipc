// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.ProcessMonitoring.Cancellation;
using ConsoLovers.Ipc.ProcessMonitoring.Result;
using ConsoLovers.Ipc.ProcessMonitoring.Services;

using Microsoft.Extensions.DependencyInjection;

public static class ServerBuilderExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds the services required for cancellation.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder UseCancellationHandler(this IServerBuilder builder)
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
   /// <param name="builder">The builder.</param>
   /// <param name="cancellationAction">The cancellation action.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder UseCancellationHandler(this IServerBuilder builder, Func<bool> cancellationAction)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));
      if (cancellationAction == null)
         throw new ArgumentNullException(nameof(cancellationAction));

      builder.UseCancellationHandler();
      builder.ConfigureService<ICancellationHandler>(x => x.OnCancellationRequested(cancellationAction));

      return builder;
   }

   /// <summary>Adds all services required for the process monitoring.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder UseProcessMonitoring(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.RemoveAspNetCoreLogging()
         .UseProgressReporter()
         .UseResultReporter()
         .UseCancellationHandler();
   }

   /// <summary>Adds the services required for reporting progress to a client.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder UseProgressReporter(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(AddRequiredServices);

      void AddRequiredServices(IServiceCollection serviceCollection)
      {
         if (serviceCollection.EnsureSingleton<IProgressReporter, ProgressReporter>())
            builder.AddGrpcService<ProgressService>();
      }

      return builder;
   }

   /// <summary>Adds the services required for reporting the detailed execution result to a client.</summary>
   /// <param name="builder">The builder.</param>
   /// <returns>The builder for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder UseResultReporter(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(AddRequiredServices);

      void AddRequiredServices(IServiceCollection serviceCollection)
      {
         if (serviceCollection.EnsureSingleton<IResultReporter, ResultReporter>())
            builder.AddGrpcService<ResultService>();
      }

      return builder;
   }

   #endregion
}
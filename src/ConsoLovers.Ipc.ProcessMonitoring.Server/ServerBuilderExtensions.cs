// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Cancellation;
using ConsoLovers.Ipc.Result;
using ConsoLovers.Ipc.Server;
using ConsoLovers.Ipc.Services;

using Microsoft.Extensions.DependencyInjection;

public static class ServerBuilderExtensions
{
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

   public static IServerBuilder UseProcessMonitoring(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.RemoveAspNetCoreLogging()
         .UseProgressReporter()
         .UseResultReporter()
         .UseCancellationHandler();
   }
}
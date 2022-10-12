// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace ConsoLovers.ConsoleToolkit.Core;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
   public static IApplicationBuilder<T> AddProcessMonitoringServer<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.AddProcessMonitoringServer(b => b.ForCurrentProcess());
   }

   public static IApplicationBuilder<T> AddProcessMonitoringServer<T>(this IApplicationBuilder<T> builder, Action<IServerBuilderWithoutName> config)
      where T : class
   {
      if (config == null)
         throw new ArgumentNullException(nameof(config));

      builder.AddIpcServer(server =>
      {
         config(server);

         var serverBuilder = (IServerBuilder)server;
         serverBuilder.AddProcessMonitoring();
      });

      builder.AddProcessMonitoringServices();
      return builder;
   }   
   
   public static IApplicationBuilder<T> AddProcessMonitoringServices<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      builder.AddService(x => x.AddSingleton(AddIpcService<IProgressReporter>));
      builder.AddService(x => x.AddSingleton(AddIpcService<IResultReporter>));
      builder.AddService(x => x.AddSingleton(AddIpcService<ICancellationHandler>));
      return builder;
   }

   private static T AddIpcService<T>(IServiceProvider serviceProvider)
      where T : notnull
   {
      var ipcServer = serviceProvider.GetRequiredService<IIpcServer>();
      return ipcServer.GetRequiredService<T>();
   }
}
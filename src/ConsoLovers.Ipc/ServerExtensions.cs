// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;

using ConsoLovers.Ipc.Result;
using ConsoLovers.Ipc.Services;

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
   /// <param name="builder">The builder without the address specified.</param>
   /// <returns>The <see cref="IServerBuilder"/></returns>
   /// <exception cref="System.ArgumentNullException">builder</exception>
   public static IServerBuilder ForCurrentProcess(this IServerBuilderWithoutAddress builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      return builder.ForProcess(Process.GetCurrentProcess());
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
   /// <returns></returns>
   /// <exception cref="System.ArgumentNullException">configureWebApplication</exception>
   /// <exception cref="System.ArgumentException">The specified {nameof(builder)} must be of type {{typeof(ServerBuilder).Name}}</exception>
   public static IServerBuilder RemoveAspNetCoreLogging(this IServerBuilder builder)
   {
      builder.ConfigureWebApplication(webApp => { webApp.Logging.ClearProviders(); });

      return builder;
   }

   public static IServerBuilder UseProgressReporter(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(x => x.AddSingleton<ProgressReporter>());
      builder.AddService(x => x.AddSingleton<IProgressReporter>(s => s.GetRequiredService<ProgressReporter>()));
      builder.AddGrpcService<ProgressService>();

      return builder;
   }

   public static IServerBuilder UseResultReporter(this IServerBuilder builder)
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(x => x.AddSingleton<ResultReporter>());
      builder.AddService(x => x.AddSingleton<IResultReporter>(s => s.GetRequiredService<ResultReporter>()));
      builder.AddGrpcService<ResultService>();

      return builder;
   }

   #endregion
}
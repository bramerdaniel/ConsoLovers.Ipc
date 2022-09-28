// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Toolkit.ProcessMonitoring.ServerExtension;

using System.Resources;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Services;
using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
   public static IApplicationBuilder<T> AddProcessMonitoringServer<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      builder.AddService(x => x.AddSingleton(CreateServerBuilder));
      builder.AddService(x => x.AddSingleton(CreateIpcServer));
      return builder;
   }

   private static IServerBuilder CreateServerBuilder(IServiceProvider services)
   {
      var serverBuilder = IpcServer.CreateServer()
         .ForCurrentProcess()
         .AddProcessMonitoring()
         .RemoveAspNetCoreLogging();

      return serverBuilder;
   }
   private static IIpcServer CreateIpcServer(IServiceProvider services)
   {
      var builder = services.GetRequiredService<IServerBuilder>();
      return builder.Start();
   }
}
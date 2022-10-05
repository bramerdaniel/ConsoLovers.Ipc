// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

using RemoteExecutableServer.Service;


public static class RemoteExecutionExtensions
{
   #region Public Methods and Operators

   public static IApplicationBuilder<T> AddRemoteExecution<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddService(s => s.AddSingleton<IIpcServerFactory, IpcServerFactory>());
      builder.AddService(s => s.AddSingleton(provider => provider.GetRequiredService<IIpcServerFactory>().Create()));
      builder.AddService(s => s.AddSingleton<IRemoteExecutionQueue, RemoteExecutionQueue>());
      builder.AddMiddleware(typeof(RemoteExecutionMiddleware<T>));

      return builder;
   }

   #endregion
}
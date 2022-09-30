// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;

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

      var executionQueue = new RemoteExecutionQueue();
      builder.AddIpcServer(c =>
      {
         c.ForName("reServer")
            .AddService(x => x.AddSingleton<IRemoteExecutionQueue>(executionQueue));

      });

      builder.AddService(s => s.AddSingleton<IRemoteExecutionQueue>(executionQueue));
      builder.AddGrpcService(typeof(RemoteExecutionService));
      builder.AddMiddleware(typeof(RemoteExecutionMiddleware<T>));

      return builder;
   }

   #endregion
}
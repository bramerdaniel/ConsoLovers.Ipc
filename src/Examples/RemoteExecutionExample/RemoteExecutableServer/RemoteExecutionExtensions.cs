// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;

using IpcServerExtension.Grpc;

public static class RemoteExecutionExtensions
{
   #region Public Methods and Operators

   public static IApplicationBuilder<T> AddRemoteExecution<T>(this IApplicationBuilder<T> builder)
      where T : class
   {
      if (builder == null)
         throw new ArgumentNullException(nameof(builder));

      builder.AddIpcServer(c => c.ForName("reServer"), false);
      builder.AddGrpcService(typeof(RemoteExecutionService));

      return builder;
   }

   #endregion
}
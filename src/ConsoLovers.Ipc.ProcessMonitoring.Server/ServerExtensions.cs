// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using Microsoft.Extensions.DependencyInjection;

public static class ServerExtensions
{
   #region Public Methods and Operators

   /// <summary>Gets the <see cref="ICancellationHandler"/> that notifies the server to cancel.</summary>
   /// <param name="server">The server.</param>
   /// <returns>The <see cref="ICancellationHandler"/> service</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static ICancellationHandler GetCancellationHandler(this IIpcServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<ICancellationHandler>();
   }

   /// <summary>Gets the <see cref="IProgressReporter"/> service.</summary>
   /// <param name="server">The <see cref="IIpcServer"/> that provided the service.</param>
   /// <returns>The <see cref="IProgressReporter"/> to use</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IProgressReporter GetProgressReporter(this IIpcServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<IProgressReporter>();
   }

   /// <summary>Gets the result reporter service.</summary>
   /// <param name="server">The server.</param>
   /// <returns>The result reporter service</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IResultReporter GetResultReporter(this IIpcServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      return server.GetRequiredService<IResultReporter>();
   }

   #endregion
}
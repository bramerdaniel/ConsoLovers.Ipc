// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Helper extensions to make server handling easier</summary>
public static class IpcServerExtensions
{
   #region Public Methods and Operators

   /// <summary>Reports progress to the <see cref="IProgressReporter"/> service.</summary>
   /// <param name="server">The server.</param>
   /// <param name="percentage">The percentage.</param>
   /// <param name="message">The message.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer ReportProgress(this IIpcServer server, int percentage, string message)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IProgressReporter>().ReportProgress(percentage, message);
      return server;
   }

   /// <summary>Reports progress to the <see cref="IProgressReporter"/> service.</summary>
   /// <param name="server">The server.</param>
   /// <param name="exitCode">The exit code of the application.</param>
   /// <param name="message">The message.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer ReportResult(this IIpcServer server, int exitCode, string message)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IResultReporter>().ReportResult(exitCode, message);
      return server;
   }

   public static IIpcServer ReportSuccess(this IIpcServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IResultReporter>().ReportSuccess();
      return server;
   }

   #endregion
}
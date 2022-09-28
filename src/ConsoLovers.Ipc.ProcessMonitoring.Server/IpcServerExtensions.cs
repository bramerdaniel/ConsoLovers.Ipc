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

   /// <summary>Adds additional data to the result, the clients could need to provide better result/error handling.</summary>
   /// <param name="server">The server.</param>
   /// <param name="key">The key.</param>
   /// <param name="value">The value.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer AddData(this IIpcServer server, string key, string value)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));
      if (key == null)
         throw new ArgumentNullException(nameof(key));
      if (value == null)
         throw new ArgumentNullException(nameof(value));

      server.GetRequiredService<IResultReporter>().AddData(key, value);
      return server;
   }

   /// <summary>Reports the error code to the clients.</summary>
   /// <param name="server">The server that had the error.</param>
   /// <param name="exitCode">The exit code of the server.</param>
   /// <param name="message">The message for the error code.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer ReportError(this IIpcServer server, int exitCode, string message)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IResultReporter>().ReportError(exitCode, message);
      return server;
   }

   /// <summary>Reports the error code to the clients.</summary>
   /// <param name="server">The server that had the error.</param>
   /// <param name="exitCode">The exit code of the server.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer ReportError(this IIpcServer server, int exitCode)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IResultReporter>().ReportError(exitCode, string.Empty);
      return server;
   }

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

   /// <summary>Notifies the clients that the server finished his work successfully.</summary>
   /// <param name="server">The server.</param>
   /// <returns>The server the method was called on</returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IIpcServer ReportSuccess(this IIpcServer server)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IResultReporter>().ReportSuccess();
      return server;
   }

   #endregion
}
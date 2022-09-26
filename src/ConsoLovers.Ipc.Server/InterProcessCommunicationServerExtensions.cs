// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterProcessCommunicationServerExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Helper extensions to make server handling easier</summary>
public static class InterProcessCommunicationServerExtensions
{
   #region Public Methods and Operators

   /// <summary>Reports progress to the <see cref="IProgressReporter"/> service.</summary>
   /// <param name="server">The server.</param>
   /// <param name="percentage">The percentage.</param>
   /// <param name="message">The message.</param>
   /// <returns></returns>
   /// <exception cref="System.ArgumentNullException">server</exception>
   public static IInterProcessCommunicationServer ReportProgress(this IInterProcessCommunicationServer server, int percentage, string message)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      server.GetRequiredService<IProgressReporter>().ReportProgress(percentage, message);
      return server;
   }

   #endregion
}
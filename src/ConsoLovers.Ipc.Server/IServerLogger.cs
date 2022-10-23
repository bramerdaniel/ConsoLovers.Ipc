// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerLogger.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IServerLogger
{
   #region Public Methods and Operators

   /// <summary>Determines whether the specified log level is enabled.</summary>
   /// <param name="logLevel">The log level to check.</param>
   /// <returns>
   ///   <c>true</c> if the specified log level is enabled; otherwise, <c>false</c>.
   /// </returns>
   bool IsEnabled(ServerLogLevel logLevel);

   /// <summary>Logs the specified message.</summary>
   /// <param name="level">The log level to use.</param>
   /// <param name="message">The message.</param>
   void Log(ServerLogLevel level, string message);

   /// <summary>Executed the <see cref="messageFunc"/> when the specified level is enabled
   /// and logs the resulting message.</summary>
   /// <param name="level">The log level.</param>
   /// <param name="messageFunc">The message function.</param>
   void Log(ServerLogLevel level, Func<string> messageFunc);

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public static class ClientLoggerExtensions
{
   #region Public Methods and Operators

   public static void Debug(this IClientLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ClientLogLevel.Debug, message);
   }

   public static void Error(this IClientLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ClientLogLevel.Error, message);
   }

   public static void Info(this IClientLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ClientLogLevel.Info, message);
   }

   public static void Trace(this IClientLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ClientLogLevel.Trace, message);
   }

   public static void Warn(this IClientLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ClientLogLevel.Warn, message);
   }

   #endregion
}
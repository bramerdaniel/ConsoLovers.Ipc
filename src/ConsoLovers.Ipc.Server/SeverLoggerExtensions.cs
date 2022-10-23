// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeverLoggerExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public static class SeverLoggerExtensions
{
   public static void Trace(this IServerLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ServerLogLevel.Trace, message);
   }  
   
   public static void Debug(this IServerLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ServerLogLevel.Debug, message);
   }

   public static void Info(this IServerLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ServerLogLevel.Info, message);
   }

   public static void Warn(this IServerLogger logger, string message)
   {
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      logger.Log(ServerLogLevel.Warn, message);
   }
}
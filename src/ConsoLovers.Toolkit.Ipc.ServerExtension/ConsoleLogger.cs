// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Toolkit.Ipc.ServerExtension;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

/// <summary><see cref="IServerLogger"/> implementation that logs to the console</summary>
/// <seealso cref="ConsoLovers.Ipc.IServerLogger" />
public class ConsoleLogger : IServerLogger
{
   #region Constructors and Destructors

   public ConsoleLogger(ServerLogLevel logLevel)
   {
      LogLevel = logLevel;
      Console = new ConsoleProxy();
   }

   #endregion

   #region IServerLogger Members

   public bool IsEnabled(ServerLogLevel logLevel)
   {
      return logLevel <= LogLevel;
   }

   public void Log(ServerLogLevel level, string message)
   {
      if (IsEnabled(level))
         LogToConsole(level, message);
   }

   public void Log(ServerLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         LogToConsole(level, messageFunc());
   }

   #endregion

   #region Public Properties

   public IConsole Console { get; set; }

   public ServerLogLevel LogLevel { get; }

   #endregion

   #region Methods

   private void LogToConsole(ServerLogLevel logLevel, string message)
   {
      switch (logLevel)
      {
         case ServerLogLevel.Off:
            break;
         case ServerLogLevel.Fatal:
            Console.WriteLine(message, ConsoleColor.DarkRed);
            break;
         case ServerLogLevel.Error:
            Console.WriteLine(message, ConsoleColor.Red);
            break;
         case ServerLogLevel.Warn:
            Console.WriteLine(message, ConsoleColor.Yellow);
            break;
         case ServerLogLevel.Info:
            Console.WriteLine(message, ConsoleColor.White);
            break;
         case ServerLogLevel.Debug:
            Console.WriteLine(message, ConsoleColor.Gray);
            break;
         case ServerLogLevel.Trace:
            Console.WriteLine(message, ConsoleColor.DarkGray);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
      }
   }

   #endregion
}
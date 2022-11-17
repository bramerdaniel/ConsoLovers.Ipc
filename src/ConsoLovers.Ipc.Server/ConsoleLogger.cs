// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary><see cref="IServerLogger"/> implementation that logs to the console</summary>
/// <seealso cref="ConsoLovers.Ipc.IServerLogger"/>
public class ConsoleLogger : IServerLogger
{
   #region Constructors and Destructors

   public ConsoleLogger(ServerLogLevel logLevel)
   {
      LogLevel = logLevel;
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
            WriteLine(logLevel, message, ConsoleColor.DarkRed);
            break;
         case ServerLogLevel.Error:
            WriteLine(logLevel, message, ConsoleColor.Red);
            break;
         case ServerLogLevel.Warn:
            WriteLine(logLevel, message, ConsoleColor.Yellow);
            break;
         case ServerLogLevel.Info:
            WriteLine(logLevel, message, ConsoleColor.White);
            break;
         case ServerLogLevel.Debug:
            WriteLine(logLevel, message, ConsoleColor.Gray);
            break;
         case ServerLogLevel.Trace:
            WriteLine(logLevel, message, ConsoleColor.DarkGray);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
      }
   }

   private static void WriteLine(ServerLogLevel logLevel, string message, ConsoleColor foregroundColor)
   {
      Console.ForegroundColor = foregroundColor;
      Console.WriteLine($"[{logLevel,-5}] {message}");
      Console.ResetColor();
   }

   #endregion
}
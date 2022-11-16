// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary><see cref="IClientLogger"/> implementation that logs to the console</summary>
/// <seealso cref="ConsoLovers.Ipc.IClientLogger"/>
public class ConsoleLogger : IClientLogger
{
   #region Constructors and Destructors

   public ConsoleLogger(ClientLogLevel logLevel)
   {
      LogLevel = logLevel;
   }

   #endregion

   #region IClientLogger Members

   public bool IsEnabled(ClientLogLevel logLevel)
   {
      return logLevel <= LogLevel;
   }

   public void Log(ClientLogLevel level, string message)
   {
      if (IsEnabled(level))
         LogToConsole(level, message);
   }

   public void Log(ClientLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         LogToConsole(level, messageFunc());
   }

   #endregion

   #region Public Properties

   public ClientLogLevel LogLevel { get; }

   #endregion

   #region Methods

   private static void WriteLine(ClientLogLevel logLevel, string message, ConsoleColor foregroundColor)
   {
      Console.ForegroundColor = foregroundColor;
      Console.WriteLine($"[{logLevel,-5}] {message}");
      Console.ResetColor();
   }

   private void LogToConsole(ClientLogLevel logLevel, string message)
   {
      switch (logLevel)
      {
         case ClientLogLevel.Off:
            break;
         case ClientLogLevel.Fatal:
            WriteLine(logLevel, message, ConsoleColor.DarkRed);
            break;
         case ClientLogLevel.Error:
            WriteLine(logLevel, message, ConsoleColor.Red);
            break;
         case ClientLogLevel.Warn:
            WriteLine(logLevel, message, ConsoleColor.Yellow);
            break;
         case ClientLogLevel.Info:
            WriteLine(logLevel, message, ConsoleColor.White);
            break;
         case ClientLogLevel.Debug:
            WriteLine(logLevel, message, ConsoleColor.Gray);
            break;
         case ClientLogLevel.Trace:
            WriteLine(logLevel, message, ConsoleColor.DarkGray);
            break;
         default:
            throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
      }
   }

   #endregion
}
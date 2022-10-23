// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>implementation that calls the specified delegate for logging</summary>
/// <seealso cref="IServerLogger"/>
public class DelegateLogger : IServerLogger
{
   #region Constants and Fields

   private readonly Action<string> logAction;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="DelegateLogger"/> class.</summary>
   /// <param name="logAction">The log action.</param>
   /// <exception cref="System.ArgumentNullException">logAction</exception>
   public DelegateLogger(Action<string> logAction)
   {
      this.logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
   }

   #endregion

   public ServerLogLevel LogLevel { get; set; } = ServerLogLevel.Warn;

   public void Log(ServerLogLevel level, string message)
   {
      if (IsEnabled(level))
         logAction(message);
   }

   public void Log(ServerLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         logAction(messageFunc());
   }

   public bool IsEnabled(ServerLogLevel logLevel)
   {
      return logLevel <= LogLevel;
   }
}
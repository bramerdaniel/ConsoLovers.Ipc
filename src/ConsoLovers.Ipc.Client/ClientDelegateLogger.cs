// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientDelegateLogger.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>implementation that calls the specified delegate for logging</summary>
/// <seealso cref="IClientLogger"/>
public class ClientDelegateLogger : IClientLogger
{
   #region Constants and Fields

   private readonly Action<string> logAction;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="DelegateLogger"/> class.</summary>
   /// <param name="logAction">The log action.</param>
   /// <exception cref="System.ArgumentNullException">logAction</exception>
   public ClientDelegateLogger(Action<string> logAction)
   {
      this.logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
   }

   #endregion

   public ClientLogLevel LogLevel { get; set; } = ClientLogLevel.Warn;

   public void Log(ClientLogLevel level, string message)
   {
      if (IsEnabled(level))
         logAction(message);
   }

   public void Log(ClientLogLevel level, Func<string> messageFunc)
   {
      if (IsEnabled(level))
         logAction(messageFunc());
   }

   public bool IsEnabled(ClientLogLevel logLevel)
   {
      return logLevel <= LogLevel;
   }
}
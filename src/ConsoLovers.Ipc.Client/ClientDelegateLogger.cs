// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientDelegateLogger.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Implementation that calls the specified delegate for logging</summary>
/// <seealso cref="IClientLogger"/>
public class ClientDelegateLogger : IClientLogger
{
   #region Constants and Fields

   private readonly Action<string> logAction;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ClientDelegateLogger"/> class.</summary>
   /// <param name="logAction">The log action.</param>
   /// <exception cref="System">logAction</exception>
   public ClientDelegateLogger(Action<string> logAction)
   {
      this.logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
   }

   #endregion

   #region IClientLogger Members

   /// <summary>Logs the specified message.</summary>
   /// <param name="message">The message to log</param>
   public void Log(string message)
   {
      logAction(message);
   }

   #endregion
}
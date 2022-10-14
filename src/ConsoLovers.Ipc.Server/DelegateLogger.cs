// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>implementation that calls the specified delegate for logging</summary>
/// <seealso cref="ConsoLovers.Ipc.IDiagnosticLogger"/>
public class DelegateLogger : IDiagnosticLogger
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

   #region IDiagnosticLogger Members

   /// <summary>Logs the specified message.</summary>
   /// <param name="message">The message to log</param>
   public void Log(string message)
   {
      logAction(message);
   }

   #endregion
}
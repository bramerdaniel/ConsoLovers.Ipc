// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagnosticLogger.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IDiagnosticLogger
{
   #region Public Methods and Operators

   /// <summary>Logs the specified message.</summary>
   void Log(string message);

   #endregion
}
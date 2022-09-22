// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Service interface that is used on the <see cref="IInterProcessCommunicationServer"/> side to report the result of the process"/></summary>
public interface IResultReporter
{
   #region Public Methods and Operators

   /// <summary>Reports the result of the host process.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   void ReportResult(int exitCode, string message);

   /// <summary>Adds the data to the result.</summary>
   /// <param name="key">The key of the data.</param>
   /// <param name="value">The value.</param>
   void AddData(string key, string value);

   /// <summary>The server process finished successfully.</summary>
   void Success();

   #endregion
}
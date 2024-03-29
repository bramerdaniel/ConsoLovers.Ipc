﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace ConsoLovers.Ipc;

using System.Globalization;

/// <summary>Service interface that is used on the <see cref="IIpcServer"/> side to report the result of the process"/></summary>
public interface IResultReporter
{
   #region Public Methods and Operators

   /// <summary>Adds the data to the result.</summary>
   /// <param name="key">The key of the data.</param>
   /// <param name="value">The value.</param>
   void AddData(string key, string value);

   /// <summary>The server process had errors while executing.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   void ReportError(int exitCode, string message);

   /// <summary>The server process had errors while executing.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The function that localizes the message.</param>
   void ReportError(int exitCode, Func<CultureInfo, string> message);

   /// <summary>Reports the result of the host process.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   void ReportResult(int exitCode, string message);

   /// <summary>Reports the result of the host process.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The function that localizes the message.</param>
   void ReportResult(int exitCode, Func<CultureInfo, string> message);

   /// <summary>The server process finished successfully without any errors.</summary>
   void ReportSuccess();

   #endregion
}
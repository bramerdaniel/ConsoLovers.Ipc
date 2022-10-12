﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace ConsoLovers.Ipc;

using System.Globalization;

/// <summary>Service interface that is used on the <see cref="IIpcServer"/> side to report progress for the current process"/></summary>
public interface IProgressReporter
{
   #region Public Methods and Operators

   /// <summary>Reports progress with a value and message.</summary>
   /// <param name="percentage">The progress value in percent.</param>
   /// <param name="message">The message.</param>
   void ReportProgress(int percentage, string message);

   void ReportProgress(Func<CultureInfo, string> message);

   void ReportProgress(int percentage, Func<CultureInfo, string> message);

   /// <summary>Reports indeterminate progress.</summary>
   /// <param name="message">The message.</param>
   void ReportProgress(string message);

   #endregion
}
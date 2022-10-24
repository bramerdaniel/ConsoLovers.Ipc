// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressReporter.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace ConsoLovers.Ipc;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

/// <summary>Service interface that is used on the <see cref="IIpcServer"/> side to report progress for the current process"/></summary>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IProgressReporter : IDisposable
{
   #region Public Properties

   /// <summary>
   ///    Gets or sets a value indicating whether the <see cref="ProgressCompleted"/> method is called automatically when the progress percentage
   ///    reaches 100 %. The default value is true
   /// </summary>
   bool AutoComplete { get; set; }

   #endregion

   #region Public Methods and Operators

   /// <summary>Notifies the clients that the progress has completed.</summary>
   void ProgressCompleted();

   /// <summary>Reports progress with a value and message.</summary>
   /// <param name="percentage">The progress value in percent.</param>
   /// <param name="message">The message.</param>
   void ReportProgress(int percentage, string message);

   /// <summary>Reports indeterminate progress.</summary>
   /// <param name="message">The function that returns the localized message for the specified culture.</param>
   void ReportProgress(Func<CultureInfo, string> message);

   /// <summary>Reports progress with a value and message.</summary>
   /// <param name="percentage">The progress value in percent.</param>
   /// <param name="message">The function that returns the localized message for the specified culture.</param>
   void ReportProgress(int percentage, Func<CultureInfo, string> message);

   /// <summary>Reports indeterminate progress.</summary>
   /// <param name="message">The message.</param>
   void ReportProgress(string message);

   #endregion
}
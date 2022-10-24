// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Result;

using System.Collections.Concurrent;
using System.Globalization;

using ConsoLovers.Ipc.Grpc;

/// <summary>The default implementation of the <see cref="IResultReporter"/> service</summary>
/// <seealso cref="ConsoLovers.Ipc.IResultReporter"/>
internal class ResultReporter : IResultReporter
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resetEvent;
   
   private readonly LocalizableResult result;

   private readonly ConcurrentDictionary<CultureInfo, ResultChangedResponse> localizedResponses;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ResultReporter"/> class.</summary>
   public ResultReporter()
   {
      resetEvent = new ManualResetEventSlim();
      result = new LocalizableResult(_ => "NotExecuted", int.MaxValue);
      localizedResponses = new ConcurrentDictionary<CultureInfo, ResultChangedResponse>();
   }

   #endregion

   #region IResultReporter Members

   /// <summary>The server process had errors while executing.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   public void ReportError(int exitCode, string message)
   {
      ReportResult(exitCode, message);
   }

   public void ReportError(int exitCode, Func<CultureInfo, string> message)
   {
      ReportResult(exitCode, message);
   }

   /// <summary>Reports the result of the host process.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   /// <exception cref="System.ArgumentNullException">message</exception>
   public void ReportResult(int exitCode, string message)
   {
      if (message == null)
         throw new ArgumentNullException(nameof(message));

      ReportResult(exitCode, _ => message);
   }

   public void ReportResult(int exitCode, Func<CultureInfo, string> message)
   {
      result.MessageResolver = message;
      result.ExitCode = exitCode;
      resetEvent.Set();
   }

   public void AddData(string key, string value)
   {
      if (key == null)
         throw new ArgumentNullException(nameof(key));
      if (value == null)
         throw new ArgumentNullException(nameof(value));

      result.Data.Add(key, value);
   }

   public void ReportSuccess()
   {
      ReportResult(0, string.Empty);
   }

   #endregion

   #region Methods

   internal Task<ResultChangedResponse> GetResultAsync(CultureInfo culture, CancellationToken cancellationToken)
   {
      resetEvent.Wait(cancellationToken);
      var response = localizedResponses.GetOrAdd(culture, c => result.Localize(c));
      return Task.FromResult(response);
   }

   #endregion
}
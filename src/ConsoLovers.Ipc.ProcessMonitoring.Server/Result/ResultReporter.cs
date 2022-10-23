// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Result;

/// <summary>The default implementation of the <see cref="IResultReporter"/> service</summary>
/// <seealso cref="ConsoLovers.Ipc.IResultReporter"/>
internal class ResultReporter : IResultReporter
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resetEvent;

   private readonly ResultInfo resultInfo;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ResultReporter"/> class.</summary>
   public ResultReporter()
   {
      resetEvent = new ManualResetEventSlim();
      resultInfo = new ResultInfo(-1, "NotExecuted");
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

   /// <summary>Reports the result of the host process.</summary>
   /// <param name="exitCode">The exit code.</param>
   /// <param name="message">The message.</param>
   /// <exception cref="System.ArgumentNullException">message</exception>
   public void ReportResult(int exitCode, string message)
   {
      if (message == null)
         throw new ArgumentNullException(nameof(message));

      resultInfo.ExitCode = exitCode;
      resultInfo.Message = message;
      resetEvent.Set();
   }

   public void AddData(string key, string value)
   {
      if (key == null)
         throw new ArgumentNullException(nameof(key));
      if (value == null)
         throw new ArgumentNullException(nameof(value));

      resultInfo.Data.Add(key, value);
   }

   public void ReportSuccess()
   {
      ReportResult(0, string.Empty);
   }

   #endregion

   #region Methods

   internal Task<ResultInfo> GetResultAsync(CancellationToken cancellationToken)
   {
      resetEvent.Wait(cancellationToken);
      return Task.FromResult(resultInfo);
   }

   #endregion
}
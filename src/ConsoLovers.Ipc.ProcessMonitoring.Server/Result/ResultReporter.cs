// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Result;

public class ResultReporter : IResultReporter
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resetEvent;

   private readonly ResultInfo resultInfo;

   #endregion

   #region Constructors and Destructors

   public ResultReporter()
   {
      resetEvent = new ManualResetEventSlim();
      resultInfo = new ResultInfo { ExitCode = -1, Message = "NotExecuted", Data = new Dictionary<string, string>() };
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

   public void ReportResult(int exitCode, string message)
   {
      resultInfo.ExitCode = exitCode;
      resultInfo.Message = message ?? string.Empty;
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

   internal Task<ResultInfo> GetResultAsync()
   {
      resetEvent.Wait();
      return Task.FromResult(resultInfo);
   }

   #endregion
}
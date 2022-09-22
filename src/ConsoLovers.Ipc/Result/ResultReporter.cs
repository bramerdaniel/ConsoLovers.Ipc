// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Result;

public class ResultReporter : IResultReporter
{
   #region Constants and Fields

   private readonly ManualResetEventSlim resetEvent;

   private ResultInfo resultInfo;

   #endregion

   #region Constructors and Destructors

   public ResultReporter()
   {
      resetEvent = new ManualResetEventSlim();
   }

   #endregion

   #region IResultReporter Members

   public void ReportResult(int exitCode, string message)
   {
      resultInfo = new ResultInfo { ExitCode = exitCode, Message = message };
      resetEvent.Set();
   }

   public void Success()
   {
      ReportResult(0, string.Empty);
   }

   #endregion

   #region Public Methods and Operators

   public Task<ResultInfo> GetResultAsync()
   {
      resetEvent.Wait();
      return Task.FromResult(resultInfo);
   }

   #endregion
}
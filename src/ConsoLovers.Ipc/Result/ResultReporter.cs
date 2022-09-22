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
      resultInfo = new ResultInfo { ExitCode = -1, Message = "NotExecuted", Data = new Dictionary<string, string>() };
   }

   #endregion

   #region IResultReporter Members

   public void ReportResult(int exitCode, string message)
   {
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
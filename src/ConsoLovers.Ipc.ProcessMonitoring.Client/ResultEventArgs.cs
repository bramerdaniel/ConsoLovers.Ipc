// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultEventArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

public class ResultEventArgs : EventArgs
{
   #region Constructors and Destructors

   public ResultEventArgs(int exitCode, string message)
   {
      Message = message;
      ExitCode = exitCode;
   }

   #endregion

   #region Public Properties

   public int ExitCode { get; }

   public string Message { get; }

   #endregion
}
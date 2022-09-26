// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressEventArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Client.Progress;

public class ProgressEventArgs
{
   #region Public Properties

   public string Message { get; set; }

   public int Percentage { get; set; }

   #endregion
}
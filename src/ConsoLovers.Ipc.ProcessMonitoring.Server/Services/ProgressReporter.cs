// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Services;

using System.Threading.Channels;

using ConsoLovers.Ipc.Grpc;

internal class ProgressReporter : IProgressReporter
{
   #region IProgressReporter Members

   public void ReportProgress(int percentage, string message)
   {
      ProgressChannel.Writer.TryWrite(new ProgressInfo { Message = message, Percentage = percentage });
   }

   public void ReportProgress(string message)
   {
      ReportProgress(-1, message ?? string.Empty);
   }

   #endregion

   #region Properties

   /// <summary>Gets the progress channel the progress is store in.</summary>
   internal Channel<ProgressInfo> ProgressChannel { get; } = Channel.CreateUnbounded<ProgressInfo>();

   #endregion
}
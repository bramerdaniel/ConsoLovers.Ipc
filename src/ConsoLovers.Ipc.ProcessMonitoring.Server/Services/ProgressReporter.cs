// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressReporter.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using System.Collections.Concurrent;
using System.Globalization;

internal class ProgressReporter : IProgressReporter
{
   #region Constants and Fields

   private ConcurrentDictionary<ClientProgressHandler, bool> clients = new ConcurrentDictionary<ClientProgressHandler, bool>();

   private ProgressTranslator? lastProgress;

   #endregion

   #region IProgressReporter Members

   public void ReportProgress(int percentage, string message)
   {
      var progressHandler = new ProgressTranslator(_ => message, percentage);
      ReportProgress(progressHandler);
   }

   public void ReportProgress(Func<CultureInfo, string> message)
   {
      ReportProgress(-1, message);
   }

   public void ReportProgress(int percentage, Func<CultureInfo, string> message)
   {
      ReportProgress(new ProgressTranslator(message, percentage));
   }

   public void ReportProgress(string message)
   {
      ReportProgress(-1, message ?? string.Empty);
   }

   #endregion

   #region Public Methods and Operators

   public void RemoveClientHandler(ClientProgressHandler clientProgressHandler)
   {
      clients.TryRemove(clientProgressHandler, out var _);
   }

   #endregion

   #region Methods

   internal ClientProgressHandler CreateClientHandler(CultureInfo culture)
   {
      var clientProgress = new ClientProgressHandler(culture);
      if (lastProgress != null)
         clientProgress.ReportProgress(lastProgress);

      clients.TryAdd(clientProgress, true);
      return clientProgress;
   }

   private void ReportProgress(ProgressTranslator progressTranslator)
   {
      lastProgress = progressTranslator;

      foreach (var channel in clients.Keys)
         channel.ReportProgress(progressTranslator);
   }

   #endregion
}
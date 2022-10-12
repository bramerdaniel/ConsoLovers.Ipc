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

   private readonly ConcurrentDictionary<ClientProgressHandler, bool> clients = new();

   private LocalizableMessage? lastProgress;

   #endregion

   #region IProgressReporter Members

   public void ReportProgress(int percentage, string message)
   {
      var progressHandler = new LocalizableMessage(_ => message, percentage);
      ReportProgress(progressHandler);
   }

   public void ReportProgress(Func<CultureInfo, string> message)
   {
      ReportProgress(-1, message);
   }

   public void ReportProgress(int percentage, Func<CultureInfo, string> message)
   {
      ReportProgress(new LocalizableMessage(message, percentage));
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

   private void ReportProgress(LocalizableMessage localizableMessage)
   {
      lastProgress = localizableMessage;

      foreach (var channel in clients.Keys)
         channel.ReportProgress(localizableMessage);
   }

   #endregion
}
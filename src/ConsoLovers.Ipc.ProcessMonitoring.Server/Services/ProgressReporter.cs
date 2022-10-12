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

   private readonly ConcurrentDictionary<CultureInfo, List<ClientProgressHandler>> clients = new();

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
      if (message == null)
         throw new ArgumentNullException(nameof(message));

      ReportProgress(new LocalizableMessage(message, percentage));
   }

   public void ReportProgress(string message)
   {
      if (message == null)
         throw new ArgumentNullException(nameof(message));
      
      ReportProgress(-1, message);
   }

   #endregion

   #region Public Methods and Operators

   public void RemoveClientHandler(ClientProgressHandler clientProgressHandler)
   {
      var culture = clientProgressHandler.Culture;
      if (clients.TryGetValue(culture, out var cultureHandlers))
      {
         cultureHandlers.Remove(clientProgressHandler);
         if (cultureHandlers.Count == 0)
            clients.TryRemove(culture, out _);
      }
   }

   #endregion

   #region Methods

   internal ClientProgressHandler CreateClientHandler(CultureInfo culture)
   {
      var cultureHandlers = clients.GetOrAdd(culture, _ => new List<ClientProgressHandler>());

      var clientProgress = new ClientProgressHandler(culture);
      cultureHandlers.Add(clientProgress);

      if (lastProgress != null)
      {
         var progressInfo = lastProgress.Localize(culture);
         clientProgress.ReportProgress(progressInfo);
      }

      return clientProgress;
   }

   private IEnumerable<(CultureInfo, List<ClientProgressHandler>)> GetGroupedHandlers()
   {
      foreach (var client in clients)
         yield return (client.Key, client.Value);
   }

   private void ReportProgress(LocalizableMessage localizableMessage)
   {
      lastProgress = localizableMessage;

      foreach (var (culture, handlers) in GetGroupedHandlers())
      {
         var progressInfo = localizableMessage.Localize(culture);
         foreach (var progressHandler in handlers)
            progressHandler.ReportProgress(progressInfo);
      }
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressReporter.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Progress;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Channels;

internal class ProgressReporter : IProgressReporter
{
   private readonly IServerLogger logger;

   #region Constants and Fields

   private readonly ConcurrentDictionary<CultureInfo, List<ClientProgressHandler>> clients = new();

   private readonly object completionLock = new();

   private bool completed;

   private LocalizableMessage? lastProgress;

   #endregion

   public ProgressReporter(IServerLogger logger)
   {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

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

   public void ProgressCompleted()
   {
      CompleteInternal();
   }

   public bool AutoComplete { get; set; } = true;

   public void Dispose()
   {
      CompleteInternal();
   }

   #endregion

   #region Public Methods and Operators

   public bool IsCompleted()
   {
      bool result;
      lock (completionLock)
         result = completed;

      return result;
   }

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
      if (IsCompleted())
         throw new ChannelClosedException();

      var cultureHandlers = clients.GetOrAdd(culture, _ => new List<ClientProgressHandler>());

      var clientProgress = new ClientProgressHandler(culture, logger);
      cultureHandlers.Add(clientProgress);

      if (lastProgress != null)
      {
         var progressInfo = lastProgress.Localize(culture);
         clientProgress.ReportProgress(progressInfo);
      }

      return clientProgress;
   }

   private void CompleteInternal()
   {
      lock (completionLock)
      {
         if (completed)
            return;

         completed = true;
      }

      foreach (var handler in clients.SelectMany(x => x.Value))
         handler.Complete();

      logger.Debug("ProgressReporter completed progress");
   }

   private IEnumerable<(CultureInfo, List<ClientProgressHandler>)> GetGroupedHandlers()
   {
      foreach (var client in clients)
         yield return (client.Key, client.Value);
   }

   private void ReportProgress(LocalizableMessage localizableMessage)
   {
      if (IsCompleted())
      {
         logger.Warn($"{nameof(ReportProgress)} was called on a completed progress reporter");
         return;
      }

      lastProgress = localizableMessage;

      foreach (var (culture, handlers) in GetGroupedHandlers())
      {
         var progressInfo = localizableMessage.Localize(culture);
         foreach (var progressHandler in handlers)
            progressHandler.ReportProgress(progressInfo);
      }

      if (AutoComplete && localizableMessage.Percentage >= 100)
         CompleteInternal();
   }

   #endregion
}
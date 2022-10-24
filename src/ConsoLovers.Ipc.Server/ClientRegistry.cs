﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientRegistry.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

internal class ClientRegistry : IClientRegistry
{
   private readonly IServerLogger logger;

   #region Constants and Fields

   private static readonly HashSet<string> AttachedClients = new();

   private readonly ManualResetEventSlim clientConnected;

   private readonly object connectionLock = new ();

   #endregion

   #region Constructors and Destructors

   public ClientRegistry(IServerLogger logger)
   {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      clientConnected = new ManualResetEventSlim();
   }

   #endregion

   #region IClientRegistry Members

   public void WaitForClient(CancellationToken cancellationToken)
   {
      logger.Info("Waiting for client to connect");
      clientConnected.Wait(cancellationToken);
   }

   public void WaitForClient(TimeSpan timeout)
   {
      var timeoutSource = new CancellationTokenSource();
      timeoutSource.CancelAfter(timeout);

      WaitForClient(timeoutSource.Token);
   }

   public void WaitForClient(int timeoutInMilliseconds)
   {
      WaitForClient(TimeSpan.FromMilliseconds(timeoutInMilliseconds));
   }

   public void WaitForClient()
   {
      WaitForClient(CancellationToken.None);
   }

   public async Task WaitForClientAsync(CancellationToken cancellationToken)
   {
      await Task.Run(() => WaitForClient(cancellationToken), cancellationToken);
   }

   public Task WaitForClientAsync(TimeSpan timeout)
   {
      var timeoutSource = new CancellationTokenSource();
      timeoutSource.CancelAfter(timeout);

      return WaitForClientAsync(timeoutSource.Token);
   }

   public Task WaitForClientAsync(int timeoutInMilliseconds)
   {
      return WaitForClientAsync(TimeSpan.FromMilliseconds(timeoutInMilliseconds));
   }

   public Task WaitForClientAsync()
   {
      return WaitForClientAsync(CancellationToken.None);
   }

   #endregion

   #region Methods

   internal void NotifyClientConnected(string name)
   {
      AddClient(name);
      NotifyInternal(name);
   }

   private void AddClient(string name)
   {
      if (string.IsNullOrWhiteSpace(name))
         return;

      if (!AttachedClients.Contains(name))
         AttachedClients.Add(name);
   }

   private void NotifyInternal(string name)
   {
      lock (connectionLock)
      {
         if (!clientConnected.IsSet)
         {
            logger.Debug($"First client (name={name}) connected");
            clientConnected.Set();
         }
      }
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientRegistry.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

// TODO Wait for a specific client (by name or by type)
public interface IClientRegistry
{
   #region Public Methods and Operators

   void WaitForClient(CancellationToken cancellationToken);

   void WaitForClient(TimeSpan timeout);

   void WaitForClient(int timeoutInMilliseconds);

   void WaitForClient();

   Task WaitForClientAsync(CancellationToken cancellationToken);

   Task WaitForClientAsync(TimeSpan timeout);

   Task WaitForClientAsync(int timeoutInMilliseconds);

   Task WaitForClientAsync();

   #endregion
}
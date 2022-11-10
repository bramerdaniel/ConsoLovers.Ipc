// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISynchronizationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>The <see cref="ISynchronizationClient"/> can be used to wait for the server to be ready to communicate</summary>
public interface ISynchronizationClient
{
   #region Public Methods and Operators

   /// <summary>Gets the name of the client.</summary>
   string Name { get; }

   /// <summary>Connects the asynchronous.</summary>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>The waiting task</returns>
   Task WaitForServerAsync(CancellationToken cancellationToken);

   /// <summary>Waits for the specified timeout for server to be available.</summary>
   /// <param name="timeout">The timeout.</param>
   /// <returns>The waiting task</returns>
   Task WaitForServerAsync(TimeSpan timeout);

   /// <summary>Waits for the specified timeout for server to be available.</summary>
   /// <param name="timeout">The timeout.</param>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>The waiting task</returns>
   Task WaitForServerAsync(TimeSpan timeout, CancellationToken cancellationToken);

   #endregion
}
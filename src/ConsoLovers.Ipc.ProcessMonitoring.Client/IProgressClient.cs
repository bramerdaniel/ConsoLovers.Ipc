// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

using ConsoLovers.Ipc.Grpc;

/// <summary>Service client for the <see cref="ProgressService"/></summary>
/// <seealso cref="IConfigurableClient"/>
/// <seealso cref="System.IDisposable"/>
public interface IProgressClient : IConfigurableClient, IDisposable
{
   #region Public Events

   /// <summary>Occurs when the reported progress of the server has changed.</summary>
   event EventHandler<ProgressEventArgs> ProgressChanged;

   /// <summary>Occurs when <see cref="State"/> property has changed.</summary>
   event EventHandler<StateChangedEventArgs> StateChanged;

   /// <summary>Waits for the progress to be completed .</summary>
   /// <returns>The waiting <see cref="Task"/></returns>
   Task WaitForCompletedAsync();

   #endregion

   #region Public Properties

   /// <summary>Gets the <see cref="Exception"/> that occurred when the state goes to <see cref="ClientState.Failed"/>.</summary>
   Exception? Exception { get; }

   /// <summary>Gets the state state of the client.</summary>
   ClientState State { get; }

   #endregion
}
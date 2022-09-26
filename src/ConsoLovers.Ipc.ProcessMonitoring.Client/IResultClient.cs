// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResultClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Client;
using ConsoLovers.Ipc.Grpc;

/// <summary>Service client for the <see cref="ResultService"/></summary>
/// <seealso cref="IConfigurableClient"/>
/// <seealso cref="System.IDisposable"/>
public interface IResultClient : IConfigurableClient, IDisposable
{
   #region Public Events

   /// <summary>Occurs when the reported result has changed. NOTE: this is only invoked once</summary>
   event EventHandler<ResultEventArgs> ResultChanged;

   /// <summary>Occurs when <see cref="State"/> property has changed.</summary>
   event EventHandler<StateChangedEventArgs> StateChanged;

   #endregion

   #region Public Properties

   /// <summary>Gets the <see cref="Exception"/> that occurred when the state goes to <see cref="ClientState.Failed"/>.</summary>
   Exception? Exception { get; }

   /// <summary>Gets the state state of the client.</summary>
   ClientState State { get; }

   #endregion

   #region Public Methods and Operators

   /// <summary>Waits for the result to be available.</summary>
   /// <returns>The <see cref="ResultInfo"/></returns>
   Task<ResultInfo> WaitForResultAsync();

   /// <summary>Waits for the result to be available.</summary>
   /// <param name="cancellationToken">The cancellation token.</param>
   /// <returns>The <see cref="ResultInfo"/></returns>
   Task<ResultInfo> WaitForResultAsync(CancellationToken cancellationToken);

   #endregion
}
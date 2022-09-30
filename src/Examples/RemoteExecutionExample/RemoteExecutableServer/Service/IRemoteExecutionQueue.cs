// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteExecutionQueue.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Service;

using System.Threading.Channels;

internal interface IRemoteExecutionQueue
{
   #region Public Properties

   Channel<RemoteJob> Jobs { get; }

   #endregion
}
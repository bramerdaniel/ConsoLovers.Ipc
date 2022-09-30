// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionQueue.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer.Service;

using System.Threading.Channels;

internal class RemoteExecutionQueue : IRemoteExecutionQueue
{
   #region Constructors and Destructors

   public RemoteExecutionQueue()
   {
      Jobs = Channel.CreateUnbounded<RemoteJob>();
      // Jobs.Writer.TryWrite(new RemoteJob("Start"));
   }

   #endregion

   #region IRemoteExecutionQueue Members

   public Channel<RemoteJob> Jobs { get; }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChannelFactory.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using global::Grpc.Net.Client;

// TODO this should be named channel info?
public interface IChannelFactory
{
   #region Public Properties

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel { get; }

   /// <summary>Gets the server name.</summary>
   string ServerName { get; }

   /// <summary>Gets the path of the socket file.</summary>
   string SocketFilePath { get; }

   /// <summary>Gets the synchronization client.</summary>
   ISynchronizationClient SynchronizationClient { get; }

   #endregion
}
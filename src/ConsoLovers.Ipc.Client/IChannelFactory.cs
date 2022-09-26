// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
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

   #endregion
}
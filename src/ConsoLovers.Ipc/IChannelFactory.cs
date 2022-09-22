// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using global::Grpc.Net.Client;

public interface IChannelFactory
{
   #region Public Properties

   /// <summary>Gets the address.</summary>
   string Address { get; }

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel { get; }

   #endregion
}
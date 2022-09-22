// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Internals;

using System.Net;
using System.Net.Sockets;

using global::Grpc.Net.Client;

/// <summary>Helper factory for creating <see cref="GrpcChannel"/>s</summary>
internal class ChannelFactory : IChannelFactory
{
   #region Constants and Fields

   private GrpcChannel? channel;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ChannelFactory"/> class.</summary>
   /// <param name="address">The address.</param>
   /// <exception cref="System.ArgumentNullException">address</exception>
   internal ChannelFactory(string address)
   {
      Address = address ?? throw new ArgumentNullException(nameof(address));
   }

   #endregion

   #region IChannelFactory Members

   /// <summary>Gets the address.</summary>
   public string Address { get; }

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel => channel ??= CreateChannel();

   #endregion

   #region Methods

   private GrpcChannel CreateChannel()
   {
      var socketPath = Path.Combine(Path.GetTempPath(), $"{Address}.uds");
      return CreateChannelFromPath(socketPath);
   }

   private GrpcChannel CreateChannelFromPath(string socketPath)
   {
      var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
      var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
      var socketsHttpHandler = new SocketsHttpHandler { ConnectCallback = connectionFactory.ConnectAsync, Proxy = new WebProxy() };
      return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions { HttpHandler = socketsHttpHandler });
   }

   #endregion
}
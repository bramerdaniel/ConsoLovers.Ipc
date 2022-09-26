// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Client;

using System.Net;
using System.Net.Sockets;

using Grpc.Net.Client;

/// <summary>Helper factory for creating <see cref="GrpcChannel"/>s</summary>
internal class ChannelFactory : IChannelFactory
{
   #region Constants and Fields

   private GrpcChannel? channel;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ChannelFactory"/> class.</summary>
   /// <param name="serverName">The serverName.</param>
   /// <exception cref="System.ArgumentNullException">serverName</exception>
   internal ChannelFactory(string serverName)
   {
      ServerName = serverName ?? throw new ArgumentNullException(nameof(serverName));
   }

   #endregion

   #region IChannelFactory Members

   /// <summary>Gets the serverName.</summary>
   public string ServerName { get; }

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel => channel ??= CreateChannel();

   #endregion

   #region Methods

   private GrpcChannel CreateChannel()
   {
      var socketPath = Path.Combine(Path.GetTempPath(), $"{ServerName}.uds");
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
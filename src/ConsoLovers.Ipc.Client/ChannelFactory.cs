// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Net;
using System.Net.Sockets;

using global::Grpc.Net.Client;


/// <summary>Helper factory for creating <see cref="GrpcChannel"/>s</summary>
internal class ChannelFactory : IChannelFactory
{
   private readonly string socketPath;

   #region Constants and Fields

   private GrpcChannel? channel;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ChannelFactory"/> class.</summary>
   /// <exception cref="System.ArgumentNullException">serverName</exception>
   internal ChannelFactory(string socketPath)
   {
      this.socketPath = socketPath ?? throw new ArgumentNullException(nameof(socketPath));
      ServerName = Path.GetFileNameWithoutExtension(socketPath);
   }

   #endregion

   #region IChannelFactory Members

   /// <summary>Gets the serverName.</summary>
   public string ServerName { get; }

   /// <summary>Gets the channel.</summary>
   public GrpcChannel Channel => channel ??= CreateChannelFromPath();

   #endregion

   #region Methods

   private GrpcChannel CreateChannelFromPath()
   {
      var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
      var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
      var socketsHttpHandler = new SocketsHttpHandler
      {
         ConnectCallback = connectionFactory.ConnectAsync,
         Proxy = new WebProxy(),
      };
      
      var grpcChannel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
      {
         HttpHandler = socketsHttpHandler
      });

      return grpcChannel;
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Net;
using System.Net.Sockets;

using ConsoLovers.Ipc.Clients;

using global::Grpc.Net.Client;

/// <summary>Helper factory for creating <see cref="GrpcChannel"/>s</summary>
internal class ChannelFactory : IChannelFactory
{
   #region Constants and Fields

   private readonly string socketPath;

   private readonly IClientLogger logger;

   private GrpcChannel? channel;

   private ISynchronizationClient? synchronizationClient;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ChannelFactory"/> class.</summary>
   /// <exception cref="System.ArgumentNullException">serverName</exception>
   internal ChannelFactory(string socketPath, IClientLogger logger)
   {
      this.socketPath = socketPath ?? throw new ArgumentNullException(nameof(socketPath));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

      ServerName = Path.GetFileNameWithoutExtension(socketPath);
   }

   #endregion

   #region IChannelFactory Members

   /// <summary>Gets the synchronization client.</summary>
   public ISynchronizationClient SynchronizationClient => synchronizationClient ??= CreateSynchronizationClient();

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
      var socketsHttpHandler = new SocketsHttpHandler { ConnectCallback = connectionFactory.ConnectAsync, Proxy = new WebProxy() };

      var grpcChannel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions { HttpHandler = socketsHttpHandler });

      return grpcChannel;
   }

   private ISynchronizationClient CreateSynchronizationClient()
   {
      return new SynchronizationClient(Channel, logger);
   }

   #endregion
}
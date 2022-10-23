// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientConfiguration.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;

using global::Grpc.Net.Client;

internal class ClientConfiguration : IClientConfiguration
{
   #region Constants and Fields

   private readonly IChannelFactory channelFactory;

   #endregion

   #region Constructors and Destructors

   public ClientConfiguration(IChannelFactory channelFactory, CultureInfo? cultureInfo)
   {
      this.channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
      Culture = cultureInfo;
   }

   #endregion

   #region IClientConfiguration Members

   public string ServerName => channelFactory.ServerName;

   public CultureInfo? Culture { get; }

   public GrpcChannel Channel => channelFactory.Channel;

   public ISynchronizationClient SynchronizationClient => channelFactory.SynchronizationClient;

   #endregion
}
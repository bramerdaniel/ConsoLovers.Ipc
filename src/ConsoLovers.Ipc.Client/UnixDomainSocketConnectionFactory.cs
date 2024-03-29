﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnixDomainSocketConnectionFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Net;
using System.Net.Sockets;

public class UnixDomainSocketConnectionFactory
{
   #region Constants and Fields

   private readonly EndPoint endPoint;

   #endregion

   #region Constructors and Destructors

   public UnixDomainSocketConnectionFactory(EndPoint endPoint)
   {
      this.endPoint = endPoint;
   }

   #endregion

   #region Public Methods and Operators

   public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _, CancellationToken cancellationToken = default)
   {
      var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
//      var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

      try
      {
         await socket.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);
         return new NetworkStream(socket, true);
      }
      catch (Exception)
      {
         socket.Dispose();
         throw;
      }
   }

   #endregion
}
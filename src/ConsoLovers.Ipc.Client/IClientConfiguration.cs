// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientConfiguration.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;

using global::Grpc.Net.Client;

public interface IClientConfiguration
{
   #region Public Properties

   /// <summary>Gets the <see cref="GrpcChannel"/> the client should use.</summary>
   public GrpcChannel Channel { get; }

   /// <summary>Gets the server name the channel was created for.</summary>
   public string ServerName { get; }

   /// <summary>Gets the <see cref="CultureInfo"/> the client runs in.</summary>
   CultureInfo? Culture { get; }

   #endregion
}
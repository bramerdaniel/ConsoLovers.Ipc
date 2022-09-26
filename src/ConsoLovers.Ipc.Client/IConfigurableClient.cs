// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurableClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Interface every client must implement to be able to be created by the <see cref="IClientFactory"/></summary>
public interface IConfigurableClient
{
   #region Public Methods and Operators

   /// <summary>Configures the client with the specified configuration.</summary>
   /// <param name="configuration">The configuration the client must use.</param>
   void Configure(IClientConfiguration configuration);

   Task ConnectAsync(CancellationToken cancellationToken);

   #endregion
}
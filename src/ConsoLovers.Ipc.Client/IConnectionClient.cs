// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IConnectionClient
{
   Task ConnectAsync(CancellationToken cancellationToken);
   
   Task ConnectAsync(TimeSpan timeout);
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICancellationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface ICancellationClient : IConfigurableClient
{
   bool RequestCancel();
}
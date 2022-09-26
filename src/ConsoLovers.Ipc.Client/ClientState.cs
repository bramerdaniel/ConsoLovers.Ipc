// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientState.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Client;

public enum ClientState
{
   Uninitialized,
   Connecting,
   Active,
   Failed,
   Closed
}
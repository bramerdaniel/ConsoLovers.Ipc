// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientState.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public enum ClientState
{
   NotConnected,
   Connecting,
   Connected,
   ConnectionClosed
}
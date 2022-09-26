// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateChangedEventArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Client;

public class StateChangedEventArgs : EventArgs
{
   #region Constructors and Destructors

   public StateChangedEventArgs(ClientState oldState, ClientState newState)
   {
      OldState = oldState;
      NewState = newState;
   }

   #endregion

   #region Public Properties

   /// <summary>Gets the new state after the change.</summary>
   public ClientState NewState { get; }

   /// <summary>Gets the old state before the change.</summary>
   public ClientState OldState { get; }

   #endregion
}
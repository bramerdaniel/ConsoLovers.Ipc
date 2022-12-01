// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerEventArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.ServerExplorer;

using System;

public class ServerEventArgs : EventArgs
{
   #region Constructors and Destructors

   public ServerEventArgs(string socketFile)
   {
      SocketFile = socketFile;
   }

   #endregion

   #region Public Properties

   public string SocketFile { get; }

   #endregion
}
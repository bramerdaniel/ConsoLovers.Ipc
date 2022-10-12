// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IServerBuilder : IServerConfiguration
{
   #region Public Methods and Operators

   /// <summary>Finishes the server setup and start it.</summary>
   /// <returns>The <see cref="IIpcServer"/></returns>
   IIpcServer Start();

   #endregion
}
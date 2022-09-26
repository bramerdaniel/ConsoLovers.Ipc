// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerBuilderWithoutName.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Server;

using System.Diagnostics;

/// <summary>An <see cref="IServerBuilder"/> without a specified server name</summary>
public interface IServerBuilderWithoutName
{
   #region Public Methods and Operators

   /// <summary>Creates the <see cref="IServerBuilder"/> for the specified unique name</summary>
   /// <param name="name">The name for the server. NOTE: this must be a valid file name,
   /// and the client must know this name</param>
   /// <returns>The server builder</returns>
   IServerBuilder ForName(string name);

   /// <summary>Creates the <see cref="IServerBuilder"/> for the specified <see cref="Process"/>.</summary>
   /// <param name="process">The process to create an <see cref="IServerBuilder"/> for.</param>
   /// <returns>The server builder</returns>
   IServerBuilder ForProcess(Process process);

   #endregion
}
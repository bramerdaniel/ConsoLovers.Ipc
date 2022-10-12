// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerBuilderWithoutName.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

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

   /// <summary>The function that computes the path to the socket file the server will use.</summary>
   /// <param name="computeSocketFile">The compute socket file.</param>
   /// <returns>The server builder</returns>
   IServerBuilder WithSocketFile(Func<string> computeSocketFile);

   /// <summary>The function that computes the path to the socket file the server will use.</summary>
   /// <param name="socketFile">The socket file path.</param>
   /// <returns>The server builder</returns>
   IServerBuilder WithSocketFile(string socketFile);

   /// <summary>Creates the <see cref="IServerBuilder"/> for the specified <see cref="Process"/>.</summary>
   /// <param name="process">The process to create an <see cref="IServerBuilder"/> for.</param>
   /// <returns>The server builder</returns>
   IServerBuilder ForProcess(Process process);

   #endregion
}
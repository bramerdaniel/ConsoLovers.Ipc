// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIpcServer.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Interface that provides access to the server side services used for inter-process communication</summary>
/// <seealso cref="System.IServiceProvider"/>
/// <seealso cref="System.IDisposable"/>
/// <seealso cref="System.IAsyncDisposable"/>
public interface IIpcServer : IServiceProvider, IDisposable, IAsyncDisposable
{
   #region Public Properties

   /// <summary>Gets the name of the server.</summary>
   string Name { get; }

   #endregion
}
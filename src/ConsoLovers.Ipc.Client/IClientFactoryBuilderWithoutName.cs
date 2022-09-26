// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientFactoryBuilderWithoutName.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Client;

using System.Diagnostics;

/// <summary>The fluent interface for the <see cref="IClientFactoryBuilder"/> when no name has ben specified</summary>
public interface IClientFactoryBuilderWithoutName
{
   #region Public Methods and Operators

   /// <summary>Creates a <see cref="IClientFactoryBuilder"/> for the specified server name</summary>
   /// <param name="name">The name of the server to create a <see cref="IClientFactoryBuilder"/> for.</param>
   /// <returns>The created <see cref="IClientFactoryBuilder"/></returns>
   IClientFactoryBuilder ForName(string name);

   /// <summary>Creates a <see cref="IClientFactoryBuilder"/> for the specified server name</summary>
   /// <param name="process">The process of the server to create a <see cref="IClientFactoryBuilder"/> for.</param>
   /// <returns>The created <see cref="IClientFactoryBuilder"/></returns>
   IClientFactoryBuilder ForProcess(Process process);

   #endregion
}
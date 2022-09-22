// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientFactoryBuilderWithoutAddress.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

/// <summary>The fluent interface for the <see cref="IClientFactoryBuilder"/> when no address has ben specified</summary>
public interface IClientFactoryBuilderWithoutAddress
{
   #region Public Methods and Operators

   IClientFactoryBuilder ForAddress(string address);

   IClientFactoryBuilder ForProcess(Process process);

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServerBuilderWithoutAddress.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

public interface IServerBuilderWithoutAddress
{
   #region Public Methods and Operators

   IServerBuilder ForAddress(string address);

   IServerBuilder ForProcess(Process process);

   #endregion
}
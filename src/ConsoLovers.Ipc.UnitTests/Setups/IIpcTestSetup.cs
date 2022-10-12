// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIpcTestSetup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Setups;

using System.Runtime.CompilerServices;

internal interface IIpcTestSetup
{
   #region Public Methods and Operators
   
   IpcTestSetup ForCurrentTest([CallerMemberName] string socketFileName = null);

   #endregion
}
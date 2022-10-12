// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Setups;

internal static class Setup
{
   #region Methods

   internal static IpcServerSetup IpcServer() => new();

   internal static IpcClientSetup IpcClient() => new();
   
   internal static IIpcTestSetup IpcTest() => new IpcTestSetup();

   #endregion
}
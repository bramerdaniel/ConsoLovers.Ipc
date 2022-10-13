// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTesting;

public static class Setup
{
   #region Methods

   public static IpcServerSetup IpcServer() => new();

   public static IpcClientSetup IpcClient() => new();

   public static IIpcTestSetup IpcTest() => new IpcTestSetup();

   #endregion
}
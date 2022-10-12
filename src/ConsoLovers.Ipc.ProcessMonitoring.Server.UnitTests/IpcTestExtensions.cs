// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcTestExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using ConsoLovers.Ipc.UnitTesting;

using Microsoft.VisualStudio.TestPlatform.TestExecutor;

internal static class IpcTestExtensions
{
   internal static IpcTestSetup AddProcessMonitoring(this IpcTestSetup testSetup)
   {
      testSetup.ServerBuilder.AddProcessMonitoring();
      testSetup.ClientFactoryBuilder.AddProcessMonitoringClients();

      return testSetup;
   }
}
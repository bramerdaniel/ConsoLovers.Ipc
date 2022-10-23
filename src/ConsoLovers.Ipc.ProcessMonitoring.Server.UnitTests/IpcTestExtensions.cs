// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcTestExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System;

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

   internal static IProgressReporter GetProgressReporter(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.Server.GetProgressReporter();
   }

   internal static IProgressClient CreateProgressClient(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.ClientFactory.CreateProgressClient();
   }

   internal static IResultClient CreateResultClient(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.ClientFactory.CreateResultClient();
   }
}
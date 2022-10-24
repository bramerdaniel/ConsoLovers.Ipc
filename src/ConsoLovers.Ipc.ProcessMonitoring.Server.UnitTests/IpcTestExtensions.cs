// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcTestExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using ConsoLovers.Ipc.UnitTesting;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal static class IpcTestExtensions
{
   #region Methods

   internal static IpcTestSetup AddProcessMonitoring(this IpcTestSetup testSetup)
   {
      testSetup.ServerBuilder.AddProcessMonitoring();
      testSetup.ClientFactoryBuilder.AddProcessMonitoringClients();

      return testSetup;
   }

   internal static IProgressClient CreateProgressClient(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.ClientFactory.CreateProgressClient();
   }

   internal static IProgressClient CreateProgressClient(this IpcTest testSetup, CultureInfo culture)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));
      if (culture == null)
         throw new ArgumentNullException(nameof(culture));

      return testSetup.ClientFactory.CreateProgressClient(culture);
   }

   internal static IProgressClient CreateProgressClient(this IpcTest testSetup, string culture)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));
      if (culture == null)
         throw new ArgumentNullException(nameof(culture));

      return testSetup.ClientFactory.CreateProgressClient(CultureInfo.GetCultureInfo(culture));
   }

   internal static IResultClient CreateResultClient(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.ClientFactory.CreateResultClient();
   }

   internal static IResultClient CreateResultClient(this IpcTest testSetup, CultureInfo culture)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.ClientFactory.CreateResultClient(culture);
   }

   internal static IProgressReporter GetProgressReporter(this IpcTest testSetup)
   {
      if (testSetup == null)
         throw new ArgumentNullException(nameof(testSetup));

      return testSetup.Server.GetProgressReporter();
   }

   #endregion
}
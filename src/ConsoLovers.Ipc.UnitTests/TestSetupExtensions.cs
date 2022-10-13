// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSetupExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests;

using ConsoLovers.Ipc.UnitTesting;
using ConsoLovers.Ipc.UnitTests.Clients;
using ConsoLovers.Ipc.UnitTests.Services;

public static class TestSetupExtensions
{
   #region Public Methods and Operators

   public static IpcTestSetup WithTestService(this IpcTestSetup testSetup)
   {
      return testSetup.WithService<UnitTestService, UnitTestClient>();
   }

   #endregion
}
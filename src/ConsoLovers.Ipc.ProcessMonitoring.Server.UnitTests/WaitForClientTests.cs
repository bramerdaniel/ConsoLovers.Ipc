// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitForClientTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using ConsoLovers.Ipc.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class WaitForClientTests
{
   #region Public Methods and Operators

   [TestMethod]
   public async Task EnsureWaitingForClientWithIntegerValueIsCanceledCorrectly()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .ConfigureClientFactory(c => c.WithDefaultCulture("de-DE"))
         .Done();

      try
      {
         await ipcTest.Server.WaitForClientAsync(10);
      }
      catch (OperationCanceledException)
      {
         return;
      }

      Assert.Fail();
   }

   [TestMethod]
   public async Task EnsureWaitingForClientWithTimeSpanValueIsCanceledCorrectly()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .ConfigureClientFactory(c => c.WithDefaultCulture("de-DE"))
         .Done();

      try
      {
         await ipcTest.Server.WaitForClientAsync(TimeSpan.FromMilliseconds(10));
      }
      catch (OperationCanceledException)
      {
         return;
      }

      Assert.Fail();
   }




   #endregion
}
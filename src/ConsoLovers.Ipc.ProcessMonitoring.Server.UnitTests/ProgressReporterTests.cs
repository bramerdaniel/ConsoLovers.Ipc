// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressReporterTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System.Threading;

using ConsoLovers.Ipc.UnitTesting;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ProgressReporterTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureProgressIsRaisedAtClientCorrectly()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.Server.GetProgressReporter();
      
      var client = ipcTest.CreateClient<IProgressClient>();
      client.WaitForServerAsync(CancellationToken.None);

      var waitHandle = new ManualResetEventSlim();

      var monitor = client.Monitor();
      client.ProgressChanged += (_, _) => waitHandle.Set();
      
      reporter.ReportProgress(100, "100 %");
      waitHandle.Wait(2000);

      monitor.Should().Raise(nameof(IProgressClient.ProgressChanged))
         .WithArgs<ProgressEventArgs>(args => args.Percentage == 100);
   }

   #endregion
}
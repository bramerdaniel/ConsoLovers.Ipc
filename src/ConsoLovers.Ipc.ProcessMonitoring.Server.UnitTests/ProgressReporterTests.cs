// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressReporterTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System;
using System.Threading;
using System.Threading.Tasks;

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

      var reporter = ipcTest.GetProgressReporter();
      
      var client = ipcTest.CreateProgressClient();
      client.WaitForServerAsync(1000);

      var waitHandle = new ManualResetEventSlim();

      var monitor = client.Monitor();
      client.ProgressChanged += (_, _) => waitHandle.Set();
      
      // we have to wait for the client progress to be created
      waitHandle.Wait(2000);

      reporter.ReportProgress(100, "100 %");
      waitHandle.Wait(2000);

      monitor.Should().Raise(nameof(IProgressClient.ProgressChanged))
         .WithArgs<ProgressEventArgs>(args => args.Percentage == 100);
   }

   [TestMethod]
   public async Task EnsureClientStateIsClosedWhenProgressReportedIsDisposed()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.GetProgressReporter();
      var client = ipcTest.CreateProgressClient();
      
      await client.WaitForServerAsync(1000);

      var waitingTask = client.WaitForCompletedAsync();
      reporter.Dispose();

      await waitingTask.WaitAsync(TimeSpan.FromMilliseconds(1000));

      client.State.Should().Be(ClientState.Closed);
   }

   [TestMethod]
   public async Task EnsureClientStateIsClosedWhenProgressReportedIsCompleted()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.GetProgressReporter();
      reporter.AutoComplete = false;

      var client = ipcTest.CreateProgressClient();

      await client.WaitForServerAsync(1000);

      var waitingTask = client.WaitForCompletedAsync();
      reporter.ProgressCompleted();

      await waitingTask;

      client.State.Should().Be(ClientState.Closed);
   }

   #endregion
}
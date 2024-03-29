﻿// --------------------------------------------------------------------------------------------------------------------
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

      await waitingTask.WaitAsync(TimeSpan.FromMilliseconds(5000));

      client.State.Should().Be(ClientState.ConnectionClosed);
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

      client.State.Should().Be(ClientState.ConnectionClosed);
   }

   [TestMethod]
   public async Task EnsureProgressCanBeLocalized()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      string germanMessage = null;
      string englishMessage = null;

      var reporter = ipcTest.GetProgressReporter();
      reporter.ReportProgress(50, c => c.Name);
      
      var germanRaised = new ManualResetEventSlim();
      var englishRaised = new ManualResetEventSlim();
      var germanClient = ipcTest.CreateProgressClient("de-DE");
      germanClient.ProgressChanged += (_, e) =>
      {
         germanMessage = e.Message;
         germanRaised.Set();
      };
      
      var englishClient = ipcTest.CreateProgressClient("en-US");
      englishClient.ProgressChanged += (_, e) =>
      {
         englishMessage = e.Message;
         englishRaised.Set();
      };

      await germanClient.WaitForServerAsync(1000);
      WaitHandle.WaitAll(new[] { englishRaised.WaitHandle, germanRaised.WaitHandle }, 2000)
         .Should().BeTrue();

      germanMessage.Should().Be("de-DE");
      englishMessage.Should().Be("en-US");
   }

   [TestMethod]
   public async Task EnsureWaitingForResultWorksCorrectlyWhenServerIsDisposed()
   {
      var server = IpcServer.CreateServer()
         .ForName("PR0001")
         .AddProgressReporter()
         .Start();

      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("PR0001")
         .AddProgressClient()
         .Build();

      server.DisposeAfter(500);

      var progressClient = clientFactory.CreateProgressClient();

      await progressClient.Invoking(async rc => await rc.WaitForCompletedAsync(10000))
         .Should().ThrowAsync<IpcException>().WithMessage("Server was shut down gracefully");
   }

   [TestMethod]
   public async Task EnsureWaitingForServerCanBeCanceledWithTimespanTimeout()
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("PR0001")
         .AddProgressClient()
         .Build();

      var progressClient = clientFactory.CreateProgressClient();
      
      await progressClient.Invoking(async client => await client.WaitForServerAsync(TimeSpan.FromMilliseconds(300)))
         .Should().ThrowAsync<OperationCanceledException>();
   }

   [TestMethod]
   public async Task EnsureWaitingForServerCanBeCanceledWithIntegerTimeout()
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("PR0002")
         .AddProgressClient()
         .Build();

      var progressClient = clientFactory.CreateProgressClient();
      
      await progressClient.Invoking(async client => await client.WaitForServerAsync(300))
         .Should().ThrowAsync<OperationCanceledException>();
   }

   [TestMethod]
   public async Task EnsureWaitingForServerCanBeDoneWithFactory()
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("PR0002")
         .AddProgressClient()
         .Build();
      
      
      await clientFactory.Invoking(async cf => await cf.WaitForServerAsync(TimeSpan.FromMilliseconds(300)))
         .Should().ThrowAsync<OperationCanceledException>();
   }

   #endregion
}
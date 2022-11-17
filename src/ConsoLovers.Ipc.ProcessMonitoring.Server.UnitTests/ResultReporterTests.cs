// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using ConsoLovers.Ipc.UnitTesting;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class ResultReporterTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureCorrectExceptionWhenServerIsNotAvailable()
   {
      var server = IpcServer.CreateServer()
         .ForName("DoesNotMatter")
         .AddResultReporter()
         .Start();

      var reporter = server.GetResultReporter();
      reporter.Invoking(x => x.ReportSuccess()).Should().NotThrow();
   }

   [TestMethod]
   public async Task EnsureCorrectResultWhenServerReportsResultCorrectly()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.Server.GetResultReporter();

      var client = ipcTest.CreateClient<IResultClient>();
      var resultTask = client.WaitForResultAsync();

      reporter.ReportResult(5, "Message");

      var result = await resultTask;
      result.ExitCode.Should().Be(5);
      result.Message.Should().Be("Message");
   }

   [TestMethod]
   public async Task EnsureCorrectResultWhenServerIsDisposedCorrectly()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.Server.GetResultReporter();
      reporter.Should().NotBeNull();

      var client = ipcTest.CreateClient<IResultClient>();
      await client.WaitForServerAsync(CancellationToken.None);

      var resultTask = client.WaitForResultAsync();

      ipcTest.Dispose();

      var result = await resultTask;
      result.ExitCode.Should().Be(int.MaxValue);
      result.Message.Should().Be("Result not computed yet");

   }

   [TestMethod]
   public async Task EnsureWaitingIsCanceledWhenServerApplicationIsStopped()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      ipcTest.Server.GetResultReporter();

      var client = ipcTest.CreateResultClient();
      await client.WaitForServerAsync(CancellationToken.None);

      var task = Task.Delay(500).ContinueWith(_ => ipcTest.StopServerApplication());
      var result = await client.WaitForResultAsync()
         .WaitAsync(TimeSpan.FromMilliseconds(5000));

      await task;

      result.ExitCode.Should().Be(int.MaxValue);
      result.Message.Should().Be("Result not computed yet");
   }


   [TestMethod]
   public async Task EnsureClientStateIsSetWhenServerIsDisposed()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      ipcTest.Server.GetResultReporter();

      var client = ipcTest.CreateResultClient();
      await client.WaitForServerAsync(500);

      client.State.Should().Be(ClientState.Connected);

      var task = Task.Delay(500).ContinueWith(_ => ipcTest.Dispose());
      await client.WaitForResultAsync()
         .WaitAsync(TimeSpan.FromMilliseconds(5000));

      await task;

      client.State.Should().Be(ClientState.ConnectionClosed);
   }

   [TestMethod]
   public async Task EnsureResultCanBeLocalized()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

      var reporter = ipcTest.Server.GetResultReporter();
      reporter.ReportResult(0, c => c.Name);

      var germanClient = ipcTest.CreateResultClient(CultureInfo.GetCultureInfo("de-DE"));
      var englishClient = ipcTest.CreateResultClient(CultureInfo.GetCultureInfo("en-US"));
      await germanClient.WaitForServerAsync(1000);

      var germanResult = await germanClient.WaitForResultAsync();
      var englishResult = await englishClient.WaitForResultAsync();

      germanResult.Message.Should().Be("de-DE");
      englishResult.Message.Should().Be("en-US");

      ipcTest.Dispose();
   }

   [TestMethod]
   public async Task EnsureResultReportingWorksCorrectly()
   {
      var ipcTest = Setup.IpcTest().ForCurrentTest()
         .AddProcessMonitoring()
         .Done();

#pragma warning disable CS4014
      Task.Delay(500).ContinueWith(_ =>
      {
         var reporter = ipcTest.Server.GetResultReporter();
         reporter.ReportResult(5, "Five");
      });
#pragma warning restore CS4014

      var client = ipcTest.ClientFactory.CreateResultClient();
      var result = await client.WaitForResultAsync();

      result.ExitCode.Should().Be(5);
      result.Message.Should().Be("Five");
   }

   [TestMethod]
   public async Task EnsureCorrectExceptionWhenWaitingHasTimedOut()
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("RR0001")
         .AddResultClient()
         .Build();
      
      var resultClient = clientFactory.CreateResultClient();

      await resultClient.Invoking(async rc => await rc.WaitForResultAsync(100))
         .Should().ThrowAsync<OperationCanceledException>();
   }

   [TestMethod]
   public async Task EnsureCorrectExceptionWhenWaitingIsCanceled()
   {
      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("RR0002")
         .AddResultClient()
         .Build();

      var resultClient = clientFactory.CreateResultClient();
      var timeoutSource = new CancellationTokenSource(200);

      await resultClient.Invoking(async rc => await rc.WaitForResultAsync(timeoutSource.Token))
         .Should().ThrowAsync<OperationCanceledException>();
   }


   [TestMethod]
   public async Task EnsureWaitingForResultWorksCorrectlyWhenServerIsDisposed()
   {
      var server = IpcServer.CreateServer()
         .ForName("RR0003")
         .AddResultReporter()
         .Start();

      var clientFactory = IpcClient.CreateClientFactory()
         .ForName("RR0003")
         .AddResultClient()
         .Build();

      server.DisposeAfter(500);

      var resultClient = clientFactory.CreateResultClient();
      var resultInfo = await resultClient.WaitForResultAsync(10000);
      Assert.Fail();
   }

   #endregion
}
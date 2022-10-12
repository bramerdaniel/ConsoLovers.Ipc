// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomServiceTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.IntegrationTests;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.UnitTesting;
using ConsoLovers.Ipc.UnitTests.Clients;
using ConsoLovers.Ipc.UnitTests.Services;

using FluentAssertions;

using global::Grpc.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class CustomServiceTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureClientFactoryUsesCultureOfCreateClientMethod()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .WithService<GreeterService, GreeterClient>()
         .Done();

      var greeterClient = ipcTest.CreateClient<GreeterClient>();
      greeterClient.SayHello("Robert").Should().Be("Hello Robert");
   }

   [TestMethod]
   public void EnsureCorrectErrorMessageWithoutCorrectServerSetup()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .Done();

      var greeterClient = ipcTest.CreateClient<GreeterClient>();
      greeterClient.Invoking(gc => gc.SayHello("Robert")).Should()
         .Throw<RpcException>().Where(e => e.StatusCode == StatusCode.Unimplemented);
   }

   [TestMethod]
   public void EnsureNoErrorWhenClientWasNotAddedToFactory()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .Done();

      ipcTest.Invoking(x => x.CreateClient<GreeterClient>()).Should()
         .NotThrow();
   }

   [TestMethod]
   public void EnsureDynamicLocalizationWorksCorrectly()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .WithService<GreeterService, GreeterClient>()
         .Done();

      var client = ipcTest.CreateClient<GreeterClient>();
      client.SayGoodby("Paul", "en-US").Should().Be("Goodby Paul");
      client.SayGoodby("Paul", "de-DE").Should().Be("Auf Wiedersehen Paul");
      client.SayGoodby("Paul", "fr-FR").Should().Be("Au revoir Paul");

   }

   #endregion
}
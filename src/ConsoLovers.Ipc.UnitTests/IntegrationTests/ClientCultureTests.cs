// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientCultureTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.IntegrationTests;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.UnitTesting;
using ConsoLovers.Ipc.UnitTests.Clients;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class ClientCultureTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureClientFactoryUsesCultureOfCreateClientMethod()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .WithTestService()
         .ConfigureClientFactory(c => c.WithDefaultCulture("de-DE"))
         .Done();
      
      var frenchClient = ipcTest.ClientFactory.CreateClient<UnitTestClient>("fr-FR");
      var localizedString = frenchClient.GetCultureName();
      localizedString.Should().Be("fr-FR");
   }

   [TestMethod]
   public void EnsureClientFactoryUsesFactoryCulture()
   {
      using var ipcTest = Setup.IpcTest()
         .ForCurrentTest()
         .WithTestService()
         .ConfigureClientFactory(c => c.WithDefaultCulture("fr-Fr"))
         .Done();
      
      var testClient = ipcTest.ClientFactory.CreateClient<UnitTestClient>();
      var localizedString = testClient.GetCultureName();
      localizedString.Should().Be("fr-FR");
   }



   #endregion
}
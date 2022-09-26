// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Client.UnitTests;

using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using global::Grpc.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class CancellationTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureCorrectExceptionWhenServerIsNotAvailable()
   {
      var factory = IpcClient.CreateClientFactory()
         .ForName("DoesNotExist")
         .AddCancellationClient()
         .Build();

      var client = factory.CreateCancellationClient();
      client.Invoking(x => x.RequestCancel()).Should().Throw<RpcException>().Where(ex => ex.StatusCode == StatusCode.Unavailable);
   }
   
   #endregion
}
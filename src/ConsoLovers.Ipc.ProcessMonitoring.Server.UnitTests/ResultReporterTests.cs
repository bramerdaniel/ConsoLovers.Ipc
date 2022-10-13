// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Server.UnitTests;

using System.Diagnostics.CodeAnalysis;

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
   
   #endregion
}
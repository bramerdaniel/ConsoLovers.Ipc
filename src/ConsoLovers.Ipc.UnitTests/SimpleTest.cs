// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingTests.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests;

using System.Diagnostics.CodeAnalysis;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class SimpleTest
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureCustomHandlerIsInvoked()
   {
   }

   [TestMethod]
   public void EnsureUseExceptionHandlerWithTypeWorksCorrectly()
   {
      Assert.Fail("This was a test only");
   }

   #endregion
}
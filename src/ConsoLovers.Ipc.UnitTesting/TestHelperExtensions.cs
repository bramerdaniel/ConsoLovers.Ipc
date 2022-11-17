// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHelperExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTesting;

using System;
using System.Threading.Tasks;

public static class TestHelperExtensions
{
   public static void DisposeAfter(this IIpcServer server, int timeoutInMilliseconds)
   {
      if (server == null)
         throw new ArgumentNullException(nameof(server));

      Task.Delay(timeoutInMilliseconds)
         .ContinueWith(_ => server.Dispose());
   }
}
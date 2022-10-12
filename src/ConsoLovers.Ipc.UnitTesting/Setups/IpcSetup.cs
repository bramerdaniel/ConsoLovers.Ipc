// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcSetup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTesting.Setups;

using System;
using System.IO;

public abstract class IpcSetup<T> : SetupBase<T>
   where T : class
{
   protected string GetSocketFile(string serverName)
   {
      if (serverName == null)
         throw new ArgumentNullException(nameof(serverName));

      var assemblyLocation = Path.GetDirectoryName(GetType().Assembly.Location);
      if (assemblyLocation == null)
         throw new InvalidOperationException("Could not resolve assembly location");

      return Path.Combine(assemblyLocation, $"{serverName}.uds");
   }
}
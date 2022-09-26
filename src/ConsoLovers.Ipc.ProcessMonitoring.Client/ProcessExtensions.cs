// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

public static class ProcessExtensions
{
   #region Public Methods and Operators

   /// <summary>Gets the name that a <see cref="IInterProcessCommunicationServer"/> of the passed process would use.</summary>
   /// <param name="process">The process.</param>
   /// <returns>The used server name</returns>
   /// <exception cref="System.ArgumentNullException">process</exception>
   public static string GetServerName(this Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));
      return $"{process.ProcessName}.{process.Id}";
   }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultInfo.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

internal record ResultInfo
{
   #region Public Properties

   public IDictionary<string, string> Data { get; internal set; }

   /// <summary>Gets or sets the exit code.</summary>
   public int ExitCode { get; internal set; }

   /// <summary>Gets or sets the message.</summary>
   public string Message { get; internal set; }

   #endregion
}
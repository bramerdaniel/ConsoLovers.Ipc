// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionResult.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;

public class RemoteExecutionResult : Dictionary<string, object>, IExecutionResult
{
   public int? ExitCode { get; set; }
}
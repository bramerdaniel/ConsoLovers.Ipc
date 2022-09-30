// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteContext.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.Middleware;

public class RemoteContext<T> : IExecutionContext<T>
   where T : class
{
   public RemoteContext(string command, RemoteExecutionResult remoteExecutionResult)
   {
      Commandline = command;
   }

   public T ApplicationArguments { get; set; }

   public object Commandline { get; set; }

   public ICommandLineArguments ParsedArguments { get; set; }

   public IExecutionResult Result { get; }
}
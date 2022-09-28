// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessMonitoringServerExtension;

using ConsoLovers.ConsoleToolkit.Core;

internal class RunCommand : IAsyncCommand<RunCommand.Args>
{
   internal class Args
   {
      
   }

   public Task ExecuteAsync(CancellationToken cancellationToken)
   {
   }

   public Args Arguments { get; set; }
}
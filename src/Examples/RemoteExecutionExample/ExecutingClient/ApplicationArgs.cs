// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ExecutingClient;

using ConsoLovers.ConsoleToolkit.Core;

using ExecutingClient.Commands;

[UsedImplicitly]
internal class ApplicationArgs
{
   #region Properties

   [Command("execute")]
   [HelpText("Executes a command on an other other process")]
   internal RemoteExecutionCommand RemoteExecution { get; set; } = null!;

   #endregion
}
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

   [Command("startServer")]
   [HelpText("Starts the server process")]
   [MenuCommand("Start server")]
   internal StartServerCommand StartServer { get; set; } = null!;

   [Command("execute")]
   [HelpText("Executes a command on an other other process")]
   [MenuCommand("Execute remote command")]
   internal RemoteExecutionCommand RemoteExecution { get; set; } = null!;

   #endregion
}
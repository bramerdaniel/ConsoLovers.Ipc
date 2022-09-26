// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using Client.Commands;
using ConsoLovers.ConsoleToolkit.Core;

internal class ClientArgs
{
   #region Public Properties

   [Command("start", "s")]
   [HelpText("Start a server")]
   public StartServerCommand StartServer{ get; set; } = null!;

   [Command("wait", "w")]
   [HelpText("Wait for a server to be connected")]
   public WaitForServerCommand WaitForServer { get; set; } = null!;

   [Command("help", "?")]
   [HelpText("Displays this help")]
   public HelpCommand Help { get; set; } = null!;

   [Argument("process", "p", Index = 0)]
   [HelpText("The name of the process to attach to")]
   public string ProcessName { get; set; } = "server";

   #endregion
}
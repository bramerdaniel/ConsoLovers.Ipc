// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client;

using ConsoLovers.ConsoleToolkit.Core;

internal class ClientArgs
{
   #region Public Properties

   [Command("help", "?")]
   [HelpText("Displays this help")]
   public HelpCommand Help { get; set; } = null!;

   [Argument("process", "p", Index = 0)]
   [HelpText("The name of the process to attach to")]
   public string ProcessName { get; set; } = "server";

   #endregion
}
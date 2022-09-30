// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;

using RemoteExecutableServer.Commands;

[UsedImplicitly]
internal class ApplicationArgs
{
   #region Properties

   [Command("start")]
   [HelpText("The start command")]
   internal StartCommand Start { get; set; } = null!;

   [Command("count")]
   [HelpText("Counts to the specified number")]
   internal CountCommand Count { get; set; } = null!;

   #endregion
}
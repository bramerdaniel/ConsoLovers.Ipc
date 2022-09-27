// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server;

using ConsoLovers.ConsoleToolkit.Core;

using Server.Annotations;
using Server.Commands;

[UsedImplicitly]
internal class ServerArgs
{
   #region Public Properties

   [Command("run", "r", IsDefaultCommand = true)]
   [HelpText("Start a ipc server and reports progress")]
   public RunCommand Run { get; set; } = null!;

   [Command("waitForCancel", "wc")]
   [HelpText("Start a ipc server and waits for a specified timeout until the server cancels the execution")]
   public AwaitCancelCommand AwaitCancel { get; set; } = null!;

   [Command("help", "?")]
   public HelpCommand Help { get; set; } = null!;

   #endregion
}
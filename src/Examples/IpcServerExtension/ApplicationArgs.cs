// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace IpcServerExtension;

using ConsoLovers.ConsoleToolkit.Core;

[UsedImplicitly]
internal class ApplicationArgs
{
   #region Properties

   [Command("run", IsDefaultCommand = true)]
   internal RunCommand Run { get; set; } = null!;

   #endregion
}
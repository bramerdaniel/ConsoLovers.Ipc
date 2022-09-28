// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationArgs.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ProcessMonitoringServerExtension;

using ConsoLovers.ConsoleToolkit.Core;

using ProcessMonitoringServerExtension.Annotations;

[UsedImplicitly]
internal class ApplicationArgs
{
   #region Properties

   [Command("run", IsDefaultCommand = true)]
   internal RunCommand Run { get; set; } = null!;

   #endregion
}
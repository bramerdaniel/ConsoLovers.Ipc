// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultInfo.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

public record ResultInfo(int ExitCode, string Message, IDictionary<string, string> Data);
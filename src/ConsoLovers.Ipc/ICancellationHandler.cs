﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICancellationHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface ICancellationHandler
{
   void OnCancellationRequested(Func<bool> action);
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProgressClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IProgressClient : IConfigurableClient, IDisposable
{
   #region Public Events

   event EventHandler<ProgressEventArgs> ProgressChanged;

   #endregion
}
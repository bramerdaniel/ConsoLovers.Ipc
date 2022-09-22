// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResultClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface IResultClient : IConfigurableClient, IDisposable
{
   #region Public Events

   event EventHandler<ResultEventArgs> ResultChanged;

   #endregion

   #region Public Methods and Operators

   Task<ResultInfo> WaitForResultAsync();

   #endregion
}
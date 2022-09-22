// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICancellationHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface ICancellationHandler
{
   #region Public Properties

   /// <summary>Gets the cancellation token of the <see cref="ICancellationHandler"/>.</summary>
   CancellationToken CancellationToken { get; }

   #endregion

   #region Public Methods and Operators

   void OnCancellationRequested(Func<bool> action);

   #endregion
}
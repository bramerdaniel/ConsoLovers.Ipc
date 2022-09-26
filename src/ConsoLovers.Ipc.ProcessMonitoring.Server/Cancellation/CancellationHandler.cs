// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Cancellation;

using ConsoLovers.Ipc.Server;

internal class CancellationHandler : ICancellationHandler
{
   #region Constants and Fields

   private readonly CancellationTokenSource handlerTokenSource = new();

   #endregion

   #region ICancellationHandler Members

   /// <summary>Gets the cancellation token of the <see cref="ICancellationHandler"/>.</summary>
   public CancellationToken CancellationToken => handlerTokenSource.Token;

   public void OnCancellationRequested(Func<bool> action)
   {
      CancellationAction = action ?? throw new ArgumentNullException(nameof(action));
   }

   #endregion

   #region Properties

   /// <summary>Gets or sets the cancellation action.</summary>
   /// <value>The cancellation action.</value>
   private Func<bool>? CancellationAction { get; set; }

   #endregion

   #region Public Methods and Operators

   public bool RequestCancel()
   {
      handlerTokenSource.Cancel();

      return CancellationAction != null && CancellationAction();
   }

   #endregion
}
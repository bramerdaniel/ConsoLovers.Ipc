// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancellationHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Cancellation;

internal class CancellationHandler : ICancellationHandler
{
   #region ICancellationHandler Members

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
      if (CancellationAction == null)
         return false;

      return CancellationAction();
   }

   #endregion
}
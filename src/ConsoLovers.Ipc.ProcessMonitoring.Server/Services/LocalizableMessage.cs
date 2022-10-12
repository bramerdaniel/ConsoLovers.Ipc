// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableMessage.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using System.Globalization;

internal class LocalizableMessage
{
   #region Constructors and Destructors

   public LocalizableMessage(Func<CultureInfo, string> messageResolver, int percentage)
   {
      MessageResolver = messageResolver ?? throw new ArgumentNullException(nameof(messageResolver));
      Percentage = percentage;
   }

   #endregion

   #region Public Properties

   public Func<CultureInfo, string> MessageResolver { get; }

   public int Percentage { get; }

   #endregion
}
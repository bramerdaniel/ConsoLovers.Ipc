// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableMessage.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using System.Globalization;

using ConsoLovers.Ipc.Grpc;

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

   public ProgressInfo Localize(CultureInfo culture)
   {
      var localizedMessage = MessageResolver(culture);
      return new ProgressInfo { Message = localizedMessage, Percentage = Percentage };
   }

   #endregion
}
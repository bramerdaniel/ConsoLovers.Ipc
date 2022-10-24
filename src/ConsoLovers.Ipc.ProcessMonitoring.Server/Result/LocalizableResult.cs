// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizableResult.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Result;

using System.Globalization;

using ConsoLovers.Ipc.Grpc;

internal class LocalizableResult
{
   #region Constructors and Destructors

   public LocalizableResult(Func<CultureInfo, string> messageResolver, int percentage)
   {
      MessageResolver = messageResolver ?? throw new ArgumentNullException(nameof(messageResolver));
      ExitCode = percentage;
      Data = new Dictionary<string, string>();
   }

   #endregion

   #region Public Properties

   public Func<CultureInfo, string> MessageResolver { get; set; }

   public int ExitCode { get; set; }

   public IDictionary<string, string> Data { get; }

   public ResultChangedResponse Localize(CultureInfo culture)
   {
      var localizedMessage = MessageResolver(culture);
      var resultInfo = new ResultChangedResponse
      {
         ExitCode = ExitCode,
         Message = localizedMessage
      };

      if (Data.Count > 0)
         resultInfo.Data.Add(Data);

      return resultInfo;
   }

   #endregion
}
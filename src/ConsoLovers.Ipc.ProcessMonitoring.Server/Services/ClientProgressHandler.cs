// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientProgressHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using System.Globalization;
using System.Threading.Channels;

using ConsoLovers.Ipc.Grpc;

internal class ClientProgressHandler
{
   internal CultureInfo Culture { get; }

   public ClientProgressHandler(CultureInfo culture)
   {
      Culture = culture ?? throw new ArgumentNullException(nameof(culture));
      ProgressChannel = Channel.CreateUnbounded<ProgressTranslator>();
   }

   private Channel<ProgressTranslator> ProgressChannel { get; }

   public void ReportProgress(ProgressTranslator progressTranslator)
   {
      ProgressChannel.Writer.TryWrite(progressTranslator);
   }

   public async Task<ProgressInfo> ReadNextAsync(CancellationToken cancellationToken)
   {
      var translator = await ProgressChannel.Reader.ReadAsync(cancellationToken);
      return new ProgressInfo
      {
         Message = translator.MessageResolver(Culture),
         Percentage = translator.Percentage
      };
   }
}
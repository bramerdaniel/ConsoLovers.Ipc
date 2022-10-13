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
      ProgressChannel = Channel.CreateUnbounded<ProgressInfo>();
   }

   private Channel<ProgressInfo> ProgressChannel { get; }

   public void ReportProgress(ProgressInfo progressInfo)
   {
      if (progressInfo == null)
         throw new ArgumentNullException(nameof(progressInfo));

      ProgressChannel.Writer.TryWrite(progressInfo);
   }

   public async Task<ProgressInfo> ReadNextAsync(CancellationToken cancellationToken)
   {
      return await ProgressChannel.Reader.ReadAsync(cancellationToken);
   }
}
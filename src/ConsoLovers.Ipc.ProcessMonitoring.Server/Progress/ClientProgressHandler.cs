// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientProgressHandler.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Progress;

using System.Globalization;
using System.Threading.Channels;

using ConsoLovers.Ipc.Grpc;

internal sealed class ClientProgressHandler
{
   #region Constants and Fields

   private readonly IServerLogger logger;

   #endregion

   #region Constructors and Destructors

   public ClientProgressHandler(CultureInfo culture, IServerLogger logger)
   {
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      Culture = culture ?? throw new ArgumentNullException(nameof(culture));
      ProgressChannel = Channel.CreateUnbounded<ProgressInfo>();
   }

   #endregion

   #region Properties

   internal CultureInfo Culture { get; }

   private Channel<ProgressInfo> ProgressChannel { get; }

   #endregion

   #region Public Methods and Operators

   public void Complete()
   {
      ProgressChannel.Writer.Complete();
   }

   public async Task<ProgressInfo> ReadNextAsync(CancellationToken cancellationToken)
   {
      return await ProgressChannel.Reader.ReadAsync(cancellationToken);
   }

   public void ReportProgress(ProgressInfo progressInfo)
   {
      if (progressInfo == null)
         throw new ArgumentNullException(nameof(progressInfo));

      if (!ProgressChannel.Writer.TryWrite(progressInfo))
         logger.Warn("Could not write to progress channel");
   }

   #endregion
}
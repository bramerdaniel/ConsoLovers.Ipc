// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Progress;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

using Microsoft.Extensions.Hosting;

internal class ProgressService : Grpc.ProgressService.ProgressServiceBase
{
   #region Constants and Fields

   private readonly ProgressReporter progressReporter;

   private readonly IServerLogger logger;

   private readonly IHostApplicationLifetime hostLifetime;

   #endregion

   #region Constructors and Destructors

   public ProgressService(ProgressReporter progressReporter, IServerLogger logger, IHostApplicationLifetime hostLifetime)
   {
      this.progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
      this.hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
      logger.Debug("Progress service was created");
   }

   #endregion

   #region Public Methods and Operators

   public override async Task ProgressChanged(ProgressChangedRequest request, IServerStreamWriter<ProgressChangedResponse> responseStream, ServerCallContext context)
   {
      var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, hostLifetime.ApplicationStopping);
      if(tokenSource.Token.IsCancellationRequested)
         return;

      var cultureInfo = context.GetCulture();
      var clientProgress = progressReporter.CreateClientHandler(cultureInfo);
      logger.Debug($"Created progress handler for culture {cultureInfo.Name}");
      
      try
      {
         while (!tokenSource.IsCancellationRequested)
         {
            var progressInfo = await clientProgress.ReadNextAsync(tokenSource.Token);
            await responseStream.WriteAsync(new ProgressChangedResponse { Progress = progressInfo }, tokenSource.Token);
            logger.Trace("Progress was reported to client");
         }
      }
      catch (System.Threading.Channels.ChannelClosedException)
      {
         // This means that there will not longer be progress as the channel for progress reporting was closed
         logger.Debug("Progress has finished");
      }
      catch (OperationCanceledException)
      {
         // call was canceled or console/web application is shutting down
         if (context.CancellationToken.IsCancellationRequested)
         {
            logger.Debug("Progress was canceled");
         }
         else
         {
            logger.Debug("Progress was canceled as the server application is shutting down");
            throw new RpcException(new Status(StatusCode.Aborted, "Server was shut down"));
         }
      }
      finally
      {
         logger.Debug($"Progress handler for culture {cultureInfo.Name} was removed");
         progressReporter.RemoveClientHandler(clientProgress);
      }
   }

   #endregion
}
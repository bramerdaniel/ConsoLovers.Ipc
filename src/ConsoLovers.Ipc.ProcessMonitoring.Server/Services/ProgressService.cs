// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

internal class ProgressService : Grpc.ProgressService.ProgressServiceBase
{
   #region Constants and Fields

   private readonly ProgressReporter progressReporter;

   private readonly IDiagnosticLogger logger;

   #endregion

   #region Constructors and Destructors

   public ProgressService(ProgressReporter progressReporter, IDiagnosticLogger logger)
   {
      this.progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
      this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   #endregion

   #region Public Methods and Operators

   public override async Task ProgressChanged(ProgressChangedRequest request, IServerStreamWriter<ProgressChangedResponse> responseStream, 
      ServerCallContext context)
   {
      var cultureInfo = context.GetCulture();
      var clientProgress = progressReporter.CreateClientHandler(cultureInfo);
      logger.Log($"Created progress handler for culture {cultureInfo.Name}");

      try
      {
         while (!context.CancellationToken.IsCancellationRequested)
         {
            var progressInfo = await clientProgress.ReadNextAsync(context.CancellationToken);
            await responseStream.WriteAsync(new ProgressChangedResponse { Progress = progressInfo });
         }
      }
      catch (OperationCanceledException)
      {
         // don't throw here
      }
      finally
      {
         logger.Log($"Progress handler for culture {cultureInfo.Name} was removed");
         progressReporter.RemoveClientHandler(clientProgress);
      }
   }

   #endregion
}
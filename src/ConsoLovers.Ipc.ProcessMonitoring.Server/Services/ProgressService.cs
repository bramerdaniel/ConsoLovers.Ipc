// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring.Services;

using System.Globalization;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

internal class ProgressService : Grpc.ProgressService.ProgressServiceBase
{
   #region Constants and Fields

   private readonly ProgressReporter progressReporter;

   #endregion

   #region Constructors and Destructors

   public ProgressService(ProgressReporter progressReporter)
   {
      this.progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));

   }

   #endregion

   #region Public Methods and Operators

   public override async Task ProgressChanged(ProgressChangedRequest request, IServerStreamWriter<ProgressChangedResponse> responseStream, 
      ServerCallContext context)
   {
      var cultureInfo = GetCulture(context);
      var clientProgress = progressReporter.CreateClientHandler(cultureInfo);
      
      try
      {
         while (!context.CancellationToken.IsCancellationRequested)
         {
            var progressInfo = await clientProgress.ReadNextAsync(context.CancellationToken);
            await responseStream.WriteAsync(new ProgressChangedResponse { Progress = progressInfo });
         }
      }
      finally
      {
         progressReporter.Remove(clientProgress);
      }
   }

   private CultureInfo GetCulture(ServerCallContext context)
   {
      // context.RequestHeaders.
      return CultureInfo.CurrentCulture;
   }

   #endregion
}
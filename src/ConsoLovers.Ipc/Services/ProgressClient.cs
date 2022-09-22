// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Services;

using System.Diagnostics.CodeAnalysis;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public sealed class ProgressClient : IProgressClient
{
   #region Constants and Fields

   private Task? progressTask;

   private Grpc.ProgressService.ProgressServiceClient? serviceClient;

   #endregion

   #region Public Events

   public event EventHandler<ProgressEventArgs>? ProgressChanged;

   #endregion

   #region IProgressClient Members

   public void Configure(IClientConfiguration configuration)
   {
      serviceClient = new Grpc.ProgressService.ProgressServiceClient(configuration.Channel);
      var changed = serviceClient.ProgressChanged(new ProgressChangedRequest());
      progressTask = Task.Run(() => ReadProgress(changed));
   }

   public void Dispose()
   {
      progressTask?.Dispose();
   }

   #endregion

   #region Methods

   private async Task ReadProgress(AsyncServerStreamingCall<ProgressChangedResponse> changed)
   {
      while (await changed.ResponseStream.MoveNext(CancellationToken.None))
      {
         var current = changed.ResponseStream.Current;
         ProgressChanged?.Invoke(this, new ProgressEventArgs { Percentage = current.Progress.Percentage, Message = current.Progress.Message });
      }
   }

   #endregion
}
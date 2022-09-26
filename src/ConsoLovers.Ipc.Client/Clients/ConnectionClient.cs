// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Clients;

using ConsoLovers.Ipc.Grpc;

using global::Grpc.Core;
using global::Grpc.Net.Client;

public class ConnectionClient : IConnectionClient
{
   #region Constants and Fields

   private readonly ConnectionService.ConnectionServiceClient connectionService;

   #endregion

   #region Constructors and Destructors

   public ConnectionClient(GrpcChannel channel)
   {
      if (channel == null)
         throw new ArgumentNullException(nameof(channel));

      connectionService = new ConnectionService.ConnectionServiceClient(channel);
   }

   #endregion

   public async Task ConnectAsync(CancellationToken cancellationToken)
   {
      await connectionService.ConnectAsync(new ConnectRequest(), null, null, cancellationToken);
   }

   public async Task ConnectAsync(TimeSpan timeout)
   {
      var tokenSource = new CancellationTokenSource();
      tokenSource.CancelAfter(timeout);

      await connectionService.ConnectAsync(new ConnectRequest(), null, null, tokenSource.Token);
   }

   public async Task WaitForConnectedAsync(CancellationToken cancellationToken)
   {
      while (true)
      {
         cancellationToken.ThrowIfCancellationRequested();

         try
         {
            await connectionService.ConnectAsync(new ConnectRequest());
            return;
         }
         catch (RpcException e)
         {
            // ConnectAsync failed
         }
      }
   }
}
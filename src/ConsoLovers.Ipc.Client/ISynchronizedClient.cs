// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISynchronizedClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public interface ISynchronizedClient
{
   /// <summary>Gets the file path to the socket file that is used.</summary>
   string SocketFilePath { get; }

   string Id { get; }

   void OnConnectionEstablished(CancellationToken cancellationToken);
   
   void OnConnectionConfirmed(CancellationToken cancellationToken);
   
   void OnConnectionAborted(CancellationToken cancellationToken);
}
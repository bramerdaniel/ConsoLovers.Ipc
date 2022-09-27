// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICancellationClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.ProcessMonitoring;

public interface ICancellationClient : IConfigurableClient
{
   /// <summary>Cancels the server execution and returns the if the cancel request was accepted.</summary>
   /// <returns>True if the cancellation was accepted, otherwise false</returns>
   bool Cancel();

   /// <summary>Cancels the server execution and returns the if the cancel request was accepted.</summary>
   /// <returns>True if the cancellation was accepted, otherwise false</returns>
   Task<bool> CancelAsync();
}
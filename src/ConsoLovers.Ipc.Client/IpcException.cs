// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcException.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using global::Grpc.Core;

public class IpcException : Exception
{
   public IpcException()
   {
   }

   public IpcException(string? message)
      : base(message)
   {
   }

   public IpcException(string? message, Exception? innerException)
      : base(message, innerException)
   {
   }

   public static IpcException FromRpcException(RpcException ex)
   {
      // This happens when the server was available and is killed without result or being disposed
      if (ex.StatusCode == StatusCode.Unavailable)
         return new IpcException("Server was terminated");

      // This happens when the server was available and is disposed without reporting any results
      if (ex.StatusCode == StatusCode.Aborted)
         return new IpcException("Server was shut down gracefully");
      
      // This happens when a requested streaming call was canceled
      if (ex.StatusCode == StatusCode.Cancelled)
         return new IpcException(ex.Message);

      // Should not happen 
      return new IpcException("Unknown error while waiting for the result", ex);
   }
}
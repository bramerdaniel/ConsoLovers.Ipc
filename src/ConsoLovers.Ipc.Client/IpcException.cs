// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcException.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

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
}
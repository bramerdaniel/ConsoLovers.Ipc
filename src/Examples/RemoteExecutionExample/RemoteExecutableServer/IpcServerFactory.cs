// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.Ipc;

public class IpcServerFactory : IIpcServerFactory
{
   private readonly IServerBuilderWithoutName serverBuilderWithoutName;
   private IServerBuilder serverBuilder;

   private string server;

   public IpcServerFactory()
   {
      serverBuilderWithoutName = IpcServer.CreateServer();
   }



   public IIpcServer Create()
   {
      return serverBuilder.Start();
   }

   public string Server
   {
      get => server;
      set
      {
         server = value;
         serverBuilder = serverBuilderWithoutName.ForName(value);
      }
   }
}
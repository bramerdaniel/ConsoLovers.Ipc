// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcTestSetup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Setups;

using System;
using System.IO;

using ConsoLovers.Ipc.UnitTests.Clients;
using ConsoLovers.Ipc.UnitTests.Services;

using Microsoft.Extensions.DependencyInjection;

internal class IpcTestSetup : SetupBase<IpcTest>, IIpcTestSetup
{
   #region IIpcTestSetup Members

   public IpcTestSetup ForCurrentTest(string socketFileName = null)
   {
      SocketPath = Path.Combine(Path.GetTempPath(), $"{socketFileName}.uds");
      ServerBuilder = IpcServer.CreateServer()
         .WithSocketFile(SocketPath)
         .RemoveAspNetCoreLogging();

      ClientFactoryBuilder = IpcClient.CreateClientFactory()
         .WithSocketFile(SocketPath);

      return this;
   }

   #endregion

   #region Public Properties

   public IClientFactoryBuilder ClientFactoryBuilder { get; set; }

   public IServerBuilder ServerBuilder { get; set; }

   #endregion

   #region Properties

   protected string SocketPath { get; private set; }

   #endregion

   #region Public Methods and Operators

   public IpcTestSetup WithTestService()
   {
      ServerBuilder.AddGrpcService<UnitTestService>();
      ClientFactoryBuilder.AddService(services => services.AddSingleton<UnitTestClient>());

      return this;
   }

   #endregion

   #region Methods

   protected override IpcTest CreateInstance()
   {
      var ipcServer = ServerBuilder.Start();
      var clientFactory = ClientFactoryBuilder.Build();

      return new IpcTest(SocketPath, ipcServer, clientFactory);
   }

   #endregion

   public IpcTestSetup ConfigureClientFactory(Action<IClientFactoryBuilder> configure)
   {
      if (configure == null)
         throw new ArgumentNullException(nameof(configure));

      configure(ClientFactoryBuilder);
      return this;
   }

   public IpcTestSetup WithService<TGrpcService, TGrpcClient>()
      where TGrpcService : class 
      where TGrpcClient : class
   {
      ServerBuilder.AddGrpcService<TGrpcService>();
      ClientFactoryBuilder.AddService(services => services.AddSingleton<TGrpcClient>());
      return this;
   }
}
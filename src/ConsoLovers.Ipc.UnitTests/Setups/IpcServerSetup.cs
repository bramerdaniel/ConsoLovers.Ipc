// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerSetup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Setups;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using ConsoLovers.Ipc.UnitTests.Services;

internal class IpcServerSetup : IpcSetup<IIpcServer>
{
   #region Properties

   internal List<Action<IServerBuilder>> SetupActions { get; set; } = new();

   private string SocketFile { get; set; }

   #endregion

   #region Public Methods and Operators

   public IpcServerSetup AddGrpcService<T>()
      where T : class
   {
      return AddService(x => x.AddGrpcService<T>());
   }

   public IpcServerSetup AddService(Action<IServerBuilder> setupAction)
   {
      SetupActions.Add(setupAction);
      return this;
   }

   public IpcServerSetup ForCurrentTest(string socketDirectory, [CallerMemberName] string serverName = null)
   {
      if (socketDirectory == null)
         throw new ArgumentNullException(nameof(socketDirectory));
      if (serverName == null)
         throw new ArgumentNullException(nameof(serverName));

      SocketFile = Path.Combine(socketDirectory, $"{serverName}.uds");
      return this;
   }

   public IpcServerSetup WithDefaults(string socketDirectory, [CallerMemberName] string serverName = null)
   {
      return ForCurrentTest(socketDirectory, serverName)
         .AddGrpcService<UnitTestService>();
   }

   #endregion

   #region Methods

   protected override IIpcServer CreateInstance()
   {
      var serverBuilder = IpcServer.CreateServer()
         .WithSocketFile(SocketFile);

      foreach (var setupAction in SetupActions)
         setupAction(serverBuilder);

      return serverBuilder.Start();
   }

   #endregion
}
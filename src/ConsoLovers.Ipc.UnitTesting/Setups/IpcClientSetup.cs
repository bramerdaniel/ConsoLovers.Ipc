// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcClientSetup.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTesting.Setups;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

public class IpcClientSetup : IpcSetup<IClientFactory>
{
   #region Constants and Fields

   private readonly List<Action<IClientFactoryBuilder>> setupActions = new();

   #endregion

   #region Public Properties

   public string SocketFile { get; set; }

   #endregion

   #region Public Methods and Operators

   public IpcClientSetup AddService<T>()
      where T : class, IConfigurableClient
   {
      setupActions.Add(fb => fb.AddService(services => services.AddSingleton<T>()));
      return this;
   }

   public IpcClientSetup Configure(Action<IClientFactoryBuilder> setupAction)
   {
      setupActions.Add(setupAction);
      return this;
   }

   public IpcClientSetup ForCurrentTest(string socketDirectory, [CallerMemberName] string serverName = null)
   {
      if (socketDirectory == null)
         throw new ArgumentNullException(nameof(socketDirectory));
      if (serverName == null)
         throw new ArgumentNullException(nameof(serverName));

      SocketFile = Path.Combine(socketDirectory, $"{serverName}.uds");
      return this;
   }

   public IpcClientSetup WithCulture(string cultureName)
   {
      var cultureInfo = CultureInfo.GetCultureInfo(cultureName);
      return Configure(x => x.WithDefaultCulture(cultureInfo));
   }

   #endregion

   #region Methods

   protected override IClientFactory CreateInstance()
   {
      var factoryBuilder = IpcClient.CreateClientFactory()
         .WithSocketFile(SocketFile);

      foreach (var setupAction in setupActions)
         setupAction(factoryBuilder);

      return factoryBuilder.Build();
   }

   #endregion
}
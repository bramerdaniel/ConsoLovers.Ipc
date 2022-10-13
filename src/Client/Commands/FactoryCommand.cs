// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Client.Commands;

using System.Globalization;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

internal class FactoryCommand
{
   #region Constructors and Destructors

   public FactoryCommand(IConsole console)
   {
      Console = console ?? throw new ArgumentNullException(nameof(console));
   }

   #endregion

   #region Public Properties

   protected IConsole Console { get; }

   #endregion

   #region Methods

   protected IClientFactory CreateFactory(string serverName, params Action<IClientFactoryBuilder>[] configuration)
   {
      var factoryBuilder = IpcClient.CreateClientFactory()
         .ForName(serverName);
      
      foreach (var configurationAction in configuration)
         configurationAction(factoryBuilder);

      System.Console.Title = $"client for {serverName}";
      Console.WriteLine($"Created client factory for sever {serverName}");
      return factoryBuilder.Build();
   }

   #endregion
}
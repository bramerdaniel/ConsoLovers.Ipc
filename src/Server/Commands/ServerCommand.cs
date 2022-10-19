// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerCommand.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Server.Commands;

using System.Diagnostics;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.Ipc;

internal class ServerCommand
{
   public ServerCommand(IConsole console)
   {
      Console = console ?? throw new ArgumentNullException(nameof(console));
   }

   protected IConsole Console { get; }

   protected IIpcServer StartServer(string? serverName,params Action<IServerBuilder>[] configureServer)
   {
      var resultingName = GetServerName(serverName);
      var serverBuilder = CreateServerBuilder(resultingName);

      foreach (var configurationAction in configureServer)
         configurationAction(serverBuilder);

      System.Console.Title = resultingName;
      Console.WriteLine($"Starting server with name {resultingName}");
      return serverBuilder.Start();
   }

   private IServerBuilder CreateServerBuilder(string serverName)
   {
      return IpcServer
         .CreateServer()
         .ForName(serverName)
         .AddDiagnosticLogging(System.Console.WriteLine)
         .RemoveAspNetCoreLogging()
         .AddGrpcReflection();
   }

   protected string GetServerName(string? serverName)
   {
      if (string.IsNullOrWhiteSpace(serverName))
      {
         var process = Process.GetCurrentProcess();
         serverName = $"{process.ProcessName}.{process.Id}";
      }

      return serverName;
   }
}
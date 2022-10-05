// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionMiddleware.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core;
using ConsoLovers.ConsoleToolkit.Core.CommandLineArguments.Parsing;
using ConsoLovers.ConsoleToolkit.Core.Middleware;
using ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

using RemoteExecutableServer.Service;

internal class RemoteExecutionMiddleware<T> : Middleware<T>
   where T : class
{
   #region Constants and Fields

   private readonly IRemoteExecutionQueue executionQueue;

   private readonly ICommandLineArgumentParser parser;


   #endregion

   #region Constructors and Destructors

   public RemoteExecutionMiddleware(IRemoteExecutionQueue executionQueue, ICommandLineArgumentParser parser)
   {
      this.executionQueue = executionQueue ?? throw new ArgumentNullException(nameof(executionQueue));
      this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
   }

   #endregion

   #region Properties

   protected override MiddlewareLocation Placement => MiddlewareLocation.BeforeParser;

   #endregion

   #region Public Methods and Operators

   public override async Task ExecuteAsync(IExecutionContext<T> context, CancellationToken cancellationToken)
   {
      if (RemoteExecutionRequested(context, out var serverName))
      {
         Console.WriteLine("Starting remote execution");

         using (var server = IpcServer.CreateServer()
                   .ForName(serverName)
                   .RemoveAspNetCoreLogging()
                   .AddService(x => x.AddSingleton(executionQueue))
                   .AddGrpcService(typeof(RemoteExecutionService))
                   .Start())
         {
            Console.WriteLine($"Server listening to {server.Name}");
            while (true)
            {
               var remoteContext = await GetNextExecutable();
               await ExecuteCommand(cancellationToken, remoteContext);
            }
         }
      }
      else
      {
         await Next(context, cancellationToken);
      }
   }

   private bool RemoteExecutionRequested(IExecutionContext<T> context, out string serverName)
   {
      serverName = string.Empty;
      var arguments = GetCommandLineArgs(context);
      if (arguments == null)
         return false;

      if (arguments.ContainsName("Remote") && arguments.TryGetValue("ServerName", out var serverArg))
      {
         if (string.IsNullOrWhiteSpace(serverArg.Value))
            return false;

         serverName = serverArg.Value;
         return true;
      }

      return false;
   }

   private ICommandLineArguments? GetCommandLineArgs(IExecutionContext<T> context)
   {
      if (context.Commandline is string commandlineString)
      {
         return parser.ParseArguments(commandlineString);
      }
      else if (context.Commandline is string[] commandlineArray)
      {
         return parser.ParseArguments(commandlineArray);
      }

      return null;
   }

   #endregion

   #region Methods

   private async Task ExecuteCommand(CancellationToken cancellationToken, IExecutionContext<T> remoteContext)
   {
      try
      {
         await Next(remoteContext, cancellationToken);
      }
      catch (Exception e)
      {
         Console.WriteLine(e.Message);
      }
   }

   private async Task<IExecutionContext<T>> GetNextExecutable()
   {
      var remoteJob = await executionQueue.Jobs.Reader.ReadAsync();
      return new RemoteContext<T>(remoteJob.Name, new RemoteExecutionResult());
   }

   #endregion
}
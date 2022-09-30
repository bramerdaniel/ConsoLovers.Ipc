// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteExecutionMiddleware.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RemoteExecutableServer;

using ConsoLovers.ConsoleToolkit.Core.Middleware;
using ConsoLovers.Ipc;

using RemoteExecutableServer.Service;

internal class RemoteExecutionMiddleware<T> : Middleware<T>
   where T : class
{
   #region Constants and Fields

   private readonly IRemoteExecutionQueue executionQueue;

   private readonly IIpcServer server;

   #endregion

   #region Constructors and Destructors

   public RemoteExecutionMiddleware(IRemoteExecutionQueue executionQueue, IIpcServer server)
   {
      this.executionQueue = executionQueue ?? throw new ArgumentNullException(nameof(executionQueue));
      this.server = server ?? throw new ArgumentNullException(nameof(server));
   }

   #endregion

   #region Properties

   protected override MiddlewareLocation Placement => MiddlewareLocation.BeforeParser;

   #endregion

   #region Public Methods and Operators

   public override async Task ExecuteAsync(IExecutionContext<T> context, CancellationToken cancellationToken)
   {
      Console.WriteLine("Starting remote execution");

      while (true)
      {
         var remoteContext = await GetNextExecutable();
         await ExecutCommand(cancellationToken, remoteContext);
      }
   }

   #endregion

   #region Methods

   private async Task ExecutCommand(CancellationToken cancellationToken, IExecutionContext<T> remoteContext)
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
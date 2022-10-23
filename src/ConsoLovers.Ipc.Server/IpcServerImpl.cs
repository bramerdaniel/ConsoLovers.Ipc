// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerImpl.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

using Microsoft.AspNetCore.Builder;

internal sealed class IpcServerImpl : IIpcServer
{
   #region Constants and Fields

   private readonly WebApplication webApplication;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="IpcServerImpl"/> class.</summary>
   /// <param name="webApplication">The web application.</param>
   /// <param name="name">The name of the server.</param>
   /// <param name="logger">The logger.</param>
   /// <exception cref="System.ArgumentNullException">name or webApplication</exception>
   /// <exception cref="ArgumentNullException">webApplication</exception>
   internal IpcServerImpl(WebApplication webApplication, string name, IDiagnosticLogger logger)
   {
      this.webApplication = webApplication ?? throw new ArgumentNullException(nameof(webApplication));
      Name = name ?? throw new ArgumentNullException(nameof(name));
      Logger = logger ?? throw new ArgumentNullException(nameof(logger));

      ServerTask = webApplication.RunAsync();
      Logger.Log($"Ipc server was started with name '{Name}'");
   }

   #endregion

   #region IIpcServer Members

   /// <summary>Gets the service.</summary>
   /// <param name="serviceType">Type of the service.</param>
   /// <returns>The requested service or null</returns>
   /// <exception cref="System.ArgumentNullException">serviceType</exception>
   public object? GetService(Type serviceType)
   {
      if (serviceType == null)
         throw new ArgumentNullException(nameof(serviceType));

      return webApplication.Services.GetService(serviceType);
   }

   /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
   public void Dispose()
   {
      DisposeAsync()
         .GetAwaiter()
         .GetResult();
   }

   /// <summary>Disposes the asynchronous.</summary>
   /// <returns>A task that represents the asynchronous dispose operation.</returns>
   public async ValueTask DisposeAsync()
   {
      Logger.Log($"Disposing ipc server '{Name}'");
      var stopwatch = Stopwatch.StartNew();

      await webApplication.StopAsync();
      await webApplication.DisposeAsync();
      await ServerTask;
      stopwatch.Stop();

      Logger.Log($"Ipc server '{Name}' disposed successfully after {stopwatch.ElapsedMilliseconds} ms.");
   }

   /// <summary>Gets the name of the server.</summary>
   public string Name { get; }

   #endregion

   #region Public Properties

   public IDiagnosticLogger Logger { get; }

   #endregion

   #region Properties

   /// <summary>Gets the server task.</summary>
   internal Task ServerTask { get; }

   #endregion
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerImpl.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Diagnostics;

using Microsoft.AspNetCore.Builder;

internal sealed class IpcServerImpl : IIpcServer
{
   #region Constants and Fields

   private readonly WebApplication webApplication;

   private bool disposed;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="IpcServerImpl"/> class.</summary>
   /// <param name="webApplication">The web application.</param>
   /// <param name="logger">The logger.</param>
   /// <param name="socketFile"></param>
   /// <exception cref="System.ArgumentNullException">name or webApplication</exception>
   /// <exception cref="ArgumentNullException">webApplication</exception>
   internal IpcServerImpl(WebApplication webApplication, IServerLogger logger, string socketFile)
   {
      this.webApplication = webApplication ?? throw new ArgumentNullException(nameof(webApplication));
      SocketFile = socketFile ?? throw new ArgumentNullException(nameof(socketFile));
      Name = Path.GetFileNameWithoutExtension(socketFile);
      Logger = logger ?? throw new ArgumentNullException(nameof(logger));

      EnsureSocketDirectory();

      ServerTask = webApplication.RunAsync();
      Logger.Info($"Ipc server was started with name '{Name}'");
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
      if (disposed)
         return;

      disposed = true;

      Logger.Debug($"Disposing ipc server '{Name}'");
      var stopwatch = Stopwatch.StartNew();

      await webApplication.StopAsync();
      Logger.Debug("Stopped the web application");

      await webApplication.DisposeAsync();
      Logger.Debug("Disposing the web application");

      TryDeleteSocketFile();

      await ServerTask;

      stopwatch.Stop();
      Logger.Info($"Ipc server '{Name}' disposed successfully after {stopwatch.ElapsedMilliseconds} ms.");
   }

   /// <summary>Gets the name of the server.</summary>
   public string Name { get; }

   #endregion

   #region Public Properties

   public IServerLogger Logger { get; }

   public string SocketFile { get; }

   #endregion

   #region Properties

   /// <summary>Gets the server task.</summary>
   internal Task ServerTask { get; }

   #endregion

   #region Methods

   private void EnsureSocketDirectory()
   {
      var directoryName = Path.GetDirectoryName(SocketFile);
      if (directoryName == null)
         throw new InvalidOperationException($"Invalid socket file path {SocketFile}");

      Directory.CreateDirectory(directoryName);
   }

   private void TryDeleteSocketFile()
   {
      try
      {
         File.Delete(SocketFile);
      }
      catch (Exception exception)
      {
         Logger.Warn($"Failed to delete socket file {SocketFile}: {exception.Message}");
      }
   }

   #endregion
}
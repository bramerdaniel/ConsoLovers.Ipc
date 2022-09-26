// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcServerImpl.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.AspNetCore.Builder;

internal sealed class IpcServerImpl : IIpcServer
{
   #region Constants and Fields

   private readonly WebApplication webApplication;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="IpcServerImpl"/> class.</summary>
   /// <param name="webApplication">The web application.</param>
   /// <exception cref="ArgumentNullException">webApplication</exception>
   internal IpcServerImpl(WebApplication webApplication)
   {
      this.webApplication = webApplication ?? throw new ArgumentNullException(nameof(webApplication));
      ServerTask = webApplication.RunAsync();
   }

   #endregion

   #region IIpcServer Members

   public object? GetService(Type serviceType)
   {
      if (serviceType == null)
         throw new ArgumentNullException(nameof(serviceType));

      return webApplication.Services.GetService(serviceType);
   }

   public void Dispose()
   {
      DisposeAsync()
         .GetAwaiter()
         .GetResult();
   }

   public async ValueTask DisposeAsync()
   {
      await webApplication.StopAsync();
      await webApplication.DisposeAsync();
      await ServerTask;
   }

   #endregion

   #region Properties

   /// <summary>Gets the server task.</summary>
   internal Task ServerTask { get; }

   #endregion
}
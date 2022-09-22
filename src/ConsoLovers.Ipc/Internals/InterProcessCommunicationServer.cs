// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterProcessCommunicationServer.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Internals;

using Microsoft.AspNetCore.Builder;

internal sealed class InterProcessCommunicationServer : IInterProcessCommunicationServer
{
   #region Constants and Fields

   private readonly WebApplication webApplication;

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="InterProcessCommunicationServer"/> class.</summary>
   /// <param name="webApplication">The web application.</param>
   /// <exception cref="System.ArgumentNullException">webApplication</exception>
   internal InterProcessCommunicationServer(WebApplication webApplication)
   {
      this.webApplication = webApplication ?? throw new ArgumentNullException(nameof(webApplication));
   }

   #endregion

   #region IInterProcessCommunicationServer Members

   public object? GetService(Type serviceType)
   {
      if (serviceType == null)
         throw new ArgumentNullException(nameof(serviceType));

      return webApplication.Services.GetService(serviceType);
   }

   public void Dispose()
   {
      ((IDisposable)webApplication).Dispose();
   }

   public async ValueTask DisposeAsync()
   {
      await webApplication.DisposeAsync();
   }

   #endregion
}
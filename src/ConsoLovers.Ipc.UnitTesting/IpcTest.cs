// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpcTest.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTesting;

using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public sealed class IpcTest : IDisposable
{
   #region Constructors and Destructors

   public IpcTest(string socketFile, IIpcServer server, IClientFactory clientFactory)
   {
      SocketFile = socketFile ?? throw new ArgumentNullException(nameof(socketFile));
      Server = server ?? throw new ArgumentNullException(nameof(server));
      ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
   }

   #endregion

   #region IDisposable Members

   public void Dispose()
   {
      Server?.Dispose();
      DeleteSocketFile();
   }

   #endregion

   #region Public Properties

   public IClientFactory ClientFactory { get; }

   public IIpcServer Server { get; }

   public string SocketFile { get; }

   #endregion

   #region Public Methods and Operators

   public T CreateClient<T>()
      where T : class, IConfigurableClient
   {
      return ClientFactory.CreateClient<T>();
   }

   #endregion

   #region Methods

   private void DeleteSocketFile()
   {
      try
      {
         if (File.Exists(SocketFile))
            File.Delete(SocketFile);
      }
      catch (IOException)
      {
         // Ignore io exceptions here
      }
   }

   #endregion

   public void StopServerApplication()
   {
      if (Server is IpcServerImpl server)
      {
         var lifetime = server.GetRequiredService<IHostApplicationLifetime>();
         lifetime.StopApplication();
      }
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using ConsoLovers.Ipc.Internals;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

internal class ServerBuilder : IServerBuilder, IServerBuilderWithoutName
{
   #region Constants and Fields

   private readonly List<Action<WebApplication>> applicationActions = new();

   #endregion

   #region Constructors and Destructors

   internal ServerBuilder()
   {
      WebApplicationBuilder = WebApplication.CreateBuilder(new WebApplicationOptions());
   }

   #endregion

   #region IServerBuilder Members

   public IServerBuilder AddService(Action<IServiceCollection> serviceSetup)
   {
      if (serviceSetup == null)
         throw new ArgumentNullException(nameof(serviceSetup));

      serviceSetup(WebApplicationBuilder.Services);
      return this;
   }

   public IServerBuilder ConfigureService(Action<IServiceProvider> serviceConfig)
   {
      if (serviceConfig == null)
         throw new ArgumentNullException(nameof(serviceConfig));

      applicationActions.Add(ConfigurationAction);
      return this;

      void ConfigurationAction(WebApplication app)
      {
         serviceConfig(app.Services);
      }
   }

   public IServerBuilder ConfigureService<T>(Action<T> serviceConfig)
      where T : class
   {
      if (serviceConfig == null)
         throw new ArgumentNullException(nameof(serviceConfig));

      applicationActions.Add(ConfigurationAction);
      return this;

      void ConfigurationAction(WebApplication app)
      {
         serviceConfig(app.Services.GetRequiredService<T>());
      }
   }

   public IServerBuilder AddGrpcService<T>()
      where T : class
   {
      AddService(x => x.AddSingleton<T>());
      applicationActions.Add(app => app.MapGrpcService<T>());
      return this;
   }

   public IIpcServer Start()
   {
      WebApplicationBuilder.Services.AddGrpc();
      
      // TODO Map GrpcReflection

      var application = WebApplicationBuilder.Build();

      foreach (var action in applicationActions)
         action(application);

      return new IpcServerImpl(application);
   }

   #endregion

   #region IServerBuilderWithoutName Members

   public IServerBuilder ForName(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      return InitializeWithName(name);
   }

   public IServerBuilder ForProcess(Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));

      return ForName(GetServerName());

      string GetServerName()
      {
         return $"{process.ProcessName}.{process.Id}";
      }
   }

   #endregion

   #region Properties

   /// <summary>Gets the web application builder.</summary>
   /// <value>The web application builder.</value>
   internal WebApplicationBuilder WebApplicationBuilder { get; }

   #endregion

   #region Methods

   private static void EnsureValidFileName(string fileName, [CallerArgumentExpression("fileName")] string? callerExpression = null)
   {
      if (fileName is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(fileName))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   private IServerBuilder InitializeWithName(string name)
   {
      EnsureValidFileName(name);

      var socketPath = Path.Combine(Path.GetTempPath(), $"{name}.uds");

      WebApplicationBuilder.WebHost.ConfigureKestrel(options =>
      {
         if (File.Exists(socketPath))
            File.Delete(socketPath);

         options.ListenUnixSocket(socketPath, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
      });

      return this;
   }

   #endregion
}
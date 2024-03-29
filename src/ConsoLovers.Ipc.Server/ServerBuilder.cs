﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

/// <summary>The default implementation of the <see cref="IServerBuilder"/> interface</summary>
/// <seealso cref="ConsoLovers.Ipc.IServerBuilder"/>
/// <seealso cref="ConsoLovers.Ipc.IServerBuilderWithoutName"/>
internal class ServerBuilder : IServerBuilder, IServerBuilderWithoutName
{
   #region Constants and Fields

   private readonly List<Action<WebApplication>> applicationActions = new();

   private Func<string> resolveSocketDirectory = ResolveSocketDirectory;

   private Func<string> resolveSocketFile = () => throw new InvalidOperationException("No socket file could be resolved");

   #endregion

   #region Constructors and Destructors

   /// <summary>Initializes a new instance of the <see cref="ServerBuilder"/> class.</summary>
   internal ServerBuilder()
   {
      WebApplicationBuilder = WebApplication.CreateBuilder(new WebApplicationOptions());
      AddGrpcService<SynchronizationService>();
      AddService(s => s.AddSingleton<ClientRegistry>());
      AddService(s => s.AddSingleton<IClientRegistry>(sp => sp.GetRequiredService<ClientRegistry>()));
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

   public IServerBuilder AddGrpcService<T>(ServiceLifetime lifetime)
      where T : class
   {
      AddService(x => x.Add(ServiceDescriptor.Describe(typeof(T), typeof(T), lifetime)));
      applicationActions.Add(app => app.MapGrpcService<T>());
      return this;
   }

   public IServerBuilder AddGrpcService(Type serviceType)
   {
      var method = typeof(ServerBuilder).GetMethods()
         .FirstOrDefault(x => x.Name == nameof(AddGrpcService) && x.IsGenericMethod);

      if (method == null)
         throw new InvalidOperationException("AddGrpcService method could not be found");

      method.MakeGenericMethod(serviceType)
         .Invoke(this, null);

      AddService(x => x.AddSingleton(serviceType));
      return this;
   }

   public IIpcServer Start()
   {
      var socketFile = resolveSocketFile();
      EnsureValidFilePath(socketFile);

      WebApplicationBuilder.WebHost.ConfigureKestrel(ConfigureKestrel);
      WebApplicationBuilder.Services.AddSingleton(Logger);
      WebApplicationBuilder.Services.AddGrpc();

      var application = WebApplicationBuilder.Build();
      foreach (var action in applicationActions)
         action(application);

      Logger.Debug("The web application was created");
      return new IpcServerImpl(application, Logger, socketFile);

      void ConfigureKestrel(KestrelServerOptions options)
      {
         if (File.Exists(socketFile))
            File.Delete(socketFile);

         Logger.Debug($"Using domain socket endpoint {socketFile}");
         options.ListenUnixSocket(socketFile, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
      }
   }

   #endregion

   #region IServerBuilderWithoutName Members

   public IServerBuilder ForName(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      EnsureValidFileName(name);
      return WithSocketFile(() => Path.Combine(resolveSocketDirectory(), $"{name}.uds"));
   }

   public IServerBuilder WithSocketDirectory(Func<string> computeSocketDirectory)
   {
      resolveSocketDirectory = computeSocketDirectory;
      return this;
   }

   public IServerBuilder WithSocketFile(Func<string> computeSocketFile)
   {
      resolveSocketFile = computeSocketFile;
      return this;
   }

   public IServerBuilder WithSocketFile(string socketFile)
   {
      return WithSocketFile(() => socketFile);
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

   /// <summary>Gets or sets the logger.</summary>
   internal IServerLogger Logger { get; set; } = new DelegateLogger(_ => { });
   
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

   private static void EnsureValidFilePath(string filePath, [CallerArgumentExpression("filePath")] string? callerExpression = null)
   {
      if (filePath is null)
         throw new ArgumentNullException(callerExpression);

      if (string.IsNullOrWhiteSpace(filePath))
         throw new ArgumentException(callerExpression, $"{callerExpression} must not be empty.");

      if (filePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
         throw new ArgumentNullException(callerExpression, $"{callerExpression} is not a valid file name.");
   }

   private static string ResolveSocketDirectory()
   {
      return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
   }

   #endregion
}
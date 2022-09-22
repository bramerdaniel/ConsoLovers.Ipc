// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBuilder.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

extern alias LoggingExtensions;
using System.Diagnostics;

using ConsoLovers.Ipc.Internals;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

internal class ServerBuilder : IServerBuilder, IServerBuilderWithoutAddress
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

   public IInterProcessCommunicationServer Start()
   {
      WebApplicationBuilder.Services.AddGrpc();
      // TODO Map GrpcReflection

      var application = WebApplicationBuilder.Build();

      foreach (var action in applicationActions)
         action(application);

      application.RunAsync();

      //// var server = application.Services.GetRequiredService<IServer>();
      //// var addressesFeature = server.Features.Get<IServerAddressesFeature>();
      //// foreach (var featureAddress in addressesFeature.Addresses)
      ////   Console.WriteLine(featureAddress);

      return new InterProcessCommunicationServer(application);
   }

   #endregion

   #region IServerBuilderWithoutAddress Members

   public IServerBuilder ForAddress(string address)
   {
      if (address == null)
         throw new ArgumentNullException(nameof(address));

      return InitializeWithAddress(address);
   }

   public IServerBuilder ForProcess(Process process)
   {
      if (process == null)
         throw new ArgumentNullException(nameof(process));

      return ForAddress(process.GetCommunicationAddress());
   }

   #endregion

   #region Properties

   /// <summary>Gets the web application builder.</summary>
   /// <value>The web application builder.</value>
   internal WebApplicationBuilder WebApplicationBuilder { get; }

   #endregion

   #region Methods

   private IServerBuilder InitializeWithAddress(string address)
   {
      var socketPath = Path.Combine(Path.GetTempPath(), $"{address}.uds");

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
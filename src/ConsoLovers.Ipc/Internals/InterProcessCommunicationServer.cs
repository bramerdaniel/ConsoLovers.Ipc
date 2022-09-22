// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterProcessCommunicationServer.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Internals;

using System.Net;
using System.Net.Sockets;

using ConsoLovers.Ipc.Services;

using global::Grpc.Net.Client;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

internal class InterProcessCommunicationServer : IInterProcessCommunicationServer
{
   #region Constants and Fields

   public static readonly string SocketPath = Path.Combine(Path.GetTempPath(), "socket.tmp");

   private readonly WebApplication webApplication;

   #endregion

   #region Constructors and Destructors

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
      webApplication.StopAsync();
      ((IDisposable)webApplication).Dispose();
   }

   #endregion

   #region Public Methods and Operators

   public static GrpcChannel CreateChannel()
   {
      var udsEndPoint = new UnixDomainSocketEndPoint(SocketPath);
      var connectionFactory = new UnixDomainSocketConnectionFactory(udsEndPoint);
      var socketsHttpHandler = new SocketsHttpHandler { ConnectCallback = connectionFactory.ConnectAsync, Proxy = new WebProxy() };
      return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions { HttpHandler = socketsHttpHandler });
   }

   public static IProgressReporter CreateProgressServer(string address)
   {
      var builder = WebApplication.CreateBuilder(new WebApplicationOptions());
      builder.WebHost.ConfigureKestrel(options =>
      {
         if (File.Exists(SocketPath))
            File.Delete(SocketPath);

         options.ListenUnixSocket(SocketPath, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
      });
      builder.Services.AddSingleton<ProgressService>();
      builder.Services.AddGrpc();
      builder.Services.AddSingleton<IProgressReporter, ProgressReporter>();
      builder.Services.AddSingleton(s => (ProgressReporter)s.GetService<IProgressReporter>());
      var application = builder.Build();
      application.MapGrpcService<ProgressService>();
      //application.RunAsync("http://[::1]:0");
      application.RunAsync();

      var server = application.Services.GetRequiredService<IServer>();

      var addressesFeature = server.Features.Get<IServerAddressesFeature>();
      foreach (var featureAddress in addressesFeature.Addresses)
         Console.WriteLine(featureAddress);

      return application.Services.GetRequiredService<IProgressReporter>();
   }

   #endregion
}
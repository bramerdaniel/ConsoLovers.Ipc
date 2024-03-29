﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurableClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using global::Grpc.Core;

/// <summary>Base class for gRPC clients that should be configured</summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ConsoLovers.Ipc.IConfigurableClient"/>
public class ConfigurableClient<T> : IConfigurableClient
{
   #region Constants and Fields

   #endregion

   #region IConfigurableClient Members

   public void Configure(IClientConfiguration configuration)
   {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      ServiceClient = (T)CreateInstance(configuration);
      SynchronizationClient = configuration.SynchronizationClient;
      OnConfigured();
   }

   private static object CreateInstance(IClientConfiguration configuration)
   {
      var clientType = typeof(T);
      var instance = Activator.CreateInstance(clientType, configuration.Channel)
                     ?? throw new InvalidOperationException($"A service client of type {clientType.Name} could not be created.");
      
      return instance;
   }

   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      await SynchronizationClient.WaitForServerAsync(cancellationToken);
   }

   #endregion

   #region Properties

   public string SocketFilePath => Configuration.SocketFilePath;

   /// <summary>Gets the configuration.</summary>
   protected IClientConfiguration Configuration { get; private set; } = null!;

   /// <summary>Gets the service client.</summary>
   protected T ServiceClient { get; private set; } = default!;

   /// <summary>Gets a synchronization client.</summary>
   protected ISynchronizationClient SynchronizationClient { get; private set; } = null!;

   #endregion

   #region Methods

   protected Metadata? AddLanguageHeader(Metadata? metadata)
   {
      if (metadata != null && Configuration.Culture != null)
         metadata.Add("Accept-Language", Configuration.Culture.Name);
      return metadata;
   }

   protected Metadata? CreateLanguageHeader()
   {
      return AddLanguageHeader(new Metadata());
   }

   protected Metadata CreateLanguageHeader(string culture)
   {
      return new Metadata { { "Accept-Language", culture } };
   }

   /// <summary>Called after the client was configured with the specified channel.</summary>
   protected virtual void OnConfigured()
   {
   }

   /// <summary>Called when the <see cref="SynchronizationClient"/> could be connected to the server</summary>
   /// <returns>The task</returns>
   protected virtual Task OnServerConnectedAsync()
   {
      return Task.CompletedTask;
   }

   #endregion
}
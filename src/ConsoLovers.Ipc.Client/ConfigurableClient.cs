// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurableClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using ConsoLovers.Ipc.Clients;

/// <summary>Base class for gRPC clients that should be configured</summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="ConsoLovers.Ipc.IConfigurableClient"/>
public class ConfigurableClient<T> : IConfigurableClient
{
   #region Constants and Fields

   private ISynchronizationClient? connectionClient;

   #endregion

   #region IConfigurableClient Members

   public void Configure(IClientConfiguration configuration)
   {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      ServiceClient = (T)Activator.CreateInstance(typeof(T), configuration.Channel);
      OnConfigured();
   }

   public async Task WaitForServerAsync(CancellationToken cancellationToken)
   {
      await SynchronizationClient.WaitForServerAsync(cancellationToken);
      await OnServerConnectedAsync();
   }

   /// <summary>Called when the <see cref="SynchronizationClient"/> could be connected to the server</summary>
   /// <returns>The task</returns>
   protected virtual Task OnServerConnectedAsync()
   {
      return Task.CompletedTask;
   }

   #endregion

   #region Properties

   /// <summary>Gets the configuration.</summary>
   protected IClientConfiguration Configuration { get; private set; } = null!;

   protected ISynchronizationClient SynchronizationClient => connectionClient ??= new SynchronizationClient(Configuration.Channel);

   /// <summary>Gets the service client.</summary>
   protected T ServiceClient { get; private set; } = default!;

   #endregion

   #region Methods

   /// <summary>Called after the client was configured with the specified channel.</summary>
   protected virtual void OnConfigured()
   {
   }

   #endregion
}
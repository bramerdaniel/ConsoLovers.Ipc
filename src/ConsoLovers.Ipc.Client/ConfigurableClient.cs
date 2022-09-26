// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurableClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.Client;

public class ConfigurableClient<T> : IConfigurableClient
{
   /// <summary>Gets the service client.</summary>
   protected T ServiceClient { get; private set; }

   public void Configure(IClientConfiguration configuration)
   {
      ServiceClient = (T)Activator.CreateInstance(typeof(T), configuration.Channel);

   }
}
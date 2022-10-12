// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientFactory.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;

public interface IClientFactory
{
   #region Public Properties

   /// <summary>Gets the channel factory the <see cref="IClientFactory"/> uses.</summary>
   IChannelFactory ChannelFactory { get; }

   #endregion

   #region Public Methods and Operators

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <returns>The created client</returns>
   T CreateClient<T>()
      where T : class, IConfigurableClient;

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <param name="culture">The culture the client will be running in.</param>
   /// <returns>The created client</returns>
   T CreateClient<T>(CultureInfo culture)
      where T : class, IConfigurableClient;

   /// <summary>Creates and configures the requested client.</summary>
   /// <typeparam name="T">The type of the client to create</typeparam>
   /// <param name="culture">The culture name the client will be running in.</param>
   /// <returns>The created client</returns>
   T CreateClient<T>(string culture)
      where T : class, IConfigurableClient;

   #endregion

   
}
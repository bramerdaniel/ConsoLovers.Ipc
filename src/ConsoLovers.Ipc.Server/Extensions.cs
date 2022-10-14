﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

/// <summary>Extension methods for the <see cref="IServerConfiguration"/></summary>
public static class Extensions
{
   #region Public Methods and Operators

   /// <summary>Adds the specified <see cref="logger"/>.</summary>
   /// <param name="configuration">The configuration to add the logger to.</param>
   /// <param name="logger">The logger.</param>
   /// <returns>The current server for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration or logger</exception>
   public static T AddDiagnosticLogging<T>(this T configuration, IDiagnosticLogger logger)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      if (configuration is ServerBuilder serverBuilder)
         serverBuilder.Logger = logger;

      return configuration;
   }

   /// <summary>Adds a <see cref="DelegateLogger"/> that calls the <see cref="logFunction"/>.</summary>
   /// <param name="configuration">The configuration to add the logger to.</param>
   /// <param name="logFunction">The log function that should be called for logging.</param>
   /// <returns>The current server for more fluent configuration</returns>
   /// <exception cref="System.ArgumentNullException">configuration or logger</exception>
   public static T AddDiagnosticLogging<T>(this T configuration, Action<string> logFunction)
      where T : IServerConfiguration
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));
      if (logFunction == null)
         throw new ArgumentNullException(nameof(logFunction));

      return configuration.AddDiagnosticLogging(new DelegateLogger(logFunction));
   }

   #endregion
}
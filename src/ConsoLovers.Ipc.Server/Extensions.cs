// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using Microsoft.Extensions.DependencyInjection;

/// <summary>Extension methods for the <see cref="IServerConfiguration"/></summary>
public static class Extensions
{
   #region Public Methods and Operators

   public static IServerConfiguration AddDiagnosticLogging(this IServerConfiguration configuration, IDiagnosticLogger logger)
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));
      if (logger == null)
         throw new ArgumentNullException(nameof(logger));

      configuration.AddService(s => s.AddSingleton(logger));
      if (configuration is ServerBuilder serverBuilder)
      {
         serverBuilder.Logger = logger;

      }

      return configuration;
   }

   public static IServerConfiguration AddDiagnosticLogging(this IServerConfiguration configuration, Action<string> logFunction)
   {
      if (configuration == null)
         throw new ArgumentNullException(nameof(configuration));
      if (logFunction == null)
         throw new ArgumentNullException(nameof(logFunction));

      return configuration.AddDiagnosticLogging(new DelegateLogger(logFunction));
   }

   #endregion
}
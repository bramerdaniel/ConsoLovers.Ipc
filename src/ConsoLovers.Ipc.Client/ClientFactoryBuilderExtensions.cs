// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryBuilderExtensions.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public static class ClientFactoryBuilderExtensions
{
   #region Public Methods and Operators

   public static IClientFactoryBuilder WithLogger(this IClientFactoryBuilder clientFactoryBuilder, Action<string> loggerFunction)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));
      if (loggerFunction == null)
         throw new ArgumentNullException(nameof(loggerFunction));

      clientFactoryBuilder.WithLogger(new ClientDelegateLogger(loggerFunction));
      return clientFactoryBuilder;
   }

   public static IClientFactoryBuilder WithLogger(this IClientFactoryBuilder clientFactoryBuilder, ClientLogLevel logLevel,
      Action<string> loggerFunction)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));
      if (loggerFunction == null)
         throw new ArgumentNullException(nameof(loggerFunction));

      clientFactoryBuilder.WithLogger(new ClientDelegateLogger(loggerFunction) { LogLevel = logLevel });
      return clientFactoryBuilder;
   }

   #endregion
}
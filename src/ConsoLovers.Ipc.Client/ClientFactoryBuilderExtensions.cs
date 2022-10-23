// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientFactoryBuilderExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public static class ClientFactoryBuilderExtensions
{
   public static IClientFactoryBuilder WithLogger(this IClientFactoryBuilder clientFactoryBuilder, Action<string> loggerFunction)
   {
      if (clientFactoryBuilder == null)
         throw new ArgumentNullException(nameof(clientFactoryBuilder));
      if (loggerFunction == null)
         throw new ArgumentNullException(nameof(loggerFunction));

      clientFactoryBuilder.WithLogger(new ClientDelegateLogger(loggerFunction));
      return clientFactoryBuilder;
   }

}
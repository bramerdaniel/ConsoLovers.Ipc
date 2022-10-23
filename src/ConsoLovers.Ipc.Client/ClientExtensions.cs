// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

public static class ClientExtensions
{
   public static Task WaitForServerAsync(this IConfigurableClient client)
   {
      if (client == null)
         throw new ArgumentNullException(nameof(client));

      return client.WaitForServerAsync(CancellationToken.None);
   }

   public static Task WaitForServerAsync(this IConfigurableClient client, TimeSpan timeout)
   {
      if (client == null)
         throw new ArgumentNullException(nameof(client));

      var source = new CancellationTokenSource();
      source.CancelAfter(timeout);

      return client.WaitForServerAsync(source.Token);
   }

   public static Task WaitForServerAsync(this IConfigurableClient client, int timeout)
   {
      if (client == null)
         throw new ArgumentNullException(nameof(client));

      var source = new CancellationTokenSource();
      source.CancelAfter(timeout);

      return client.WaitForServerAsync(source.Token);
   }
}
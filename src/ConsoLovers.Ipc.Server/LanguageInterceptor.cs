// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageInterceptor.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc;

using System.Globalization;

using global::Grpc.Core;
using global::Grpc.Core.Interceptors;

using Microsoft.Net.Http.Headers;

[Obsolete]
internal class LanguageInterceptor : Interceptor
{
   #region Public Methods and Operators

   public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
      ServerCallContext context,
      ClientStreamingServerMethod<TRequest, TResponse> continuation)
   {
      Console.WriteLine("ClientStreamingServerHandler");
      return base.ClientStreamingServerHandler(requestStream, context, continuation);
   }

   public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream,
      ServerCallContext context,
      ServerStreamingServerMethod<TRequest, TResponse> continuation)
   {
      var headerLanguage = context.RequestHeaders.FirstOrDefault(t => t.Key == HeaderNames.AcceptLanguage);
      if (headerLanguage != null)
      {
         Thread.CurrentThread.CurrentCulture = new CultureInfo(headerLanguage.Value);
         Thread.CurrentThread.CurrentUICulture = new CultureInfo(headerLanguage.Value);
      }

      return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
   }

   #endregion
}
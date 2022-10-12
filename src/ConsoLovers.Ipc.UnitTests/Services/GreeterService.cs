// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GreeterService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Services;

using System.Threading.Tasks;

using ConsoLovers.Ipc.UnitTests.Grpc;

using global::Grpc.Core;

internal class GreeterService : Grpc.GreeterService.GreeterServiceBase
{
   #region Public Methods and Operators

   public override Task<SayGoodbyResponse> SayGoodby(SayGoodbyRequest request, ServerCallContext context)
   {
      var name = context.GetCulture().Name;
      var message = GetMessage(request.Name, name);

      return Task.FromResult(new SayGoodbyResponse { Message = message });
   }

   public override Task<SayHelloResponse> SayHello(SayHelloRequest request, ServerCallContext context)
   {
      return Task.FromResult(new SayHelloResponse { Message = $"Hello {request.Name}" });
   }

   #endregion

   #region Methods
   
   private static string GetMessage(string name, string culture)
   {
      return culture switch
      {
         "de-DE" => $"Auf Wiedersehen {name}",
         "fr-FR" => $"Au revoir {name}",
         _ => $"Goodby {name}"
      };
   }

   #endregion
}
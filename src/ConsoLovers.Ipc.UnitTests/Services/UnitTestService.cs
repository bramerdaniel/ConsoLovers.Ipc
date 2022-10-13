// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestService.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Services;

using System.Threading.Tasks;

using ConsoLovers.Ipc.UnitTests.Grpc;

using global::Grpc.Core;

internal class UnitTestService : Grpc.UnitTestService.UnitTestServiceBase
{
   public override Task<GetCultureNameResponse> GetCultureName(GetCultureNameRequest request, ServerCallContext context)
   {
      var culture = context.GetCulture();
      return Task.FromResult(new GetCultureNameResponse { CultureName = culture.Name });
   }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTestClient.cs" company="ConsoLovers">
//    Copyright (c) ConsoLovers  2015 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ConsoLovers.Ipc.UnitTests.Clients;

using ConsoLovers.Ipc.UnitTests.Grpc;

internal class UnitTestClient : ConfigurableClient<UnitTestService.UnitTestServiceClient>
{
   internal string GetCultureName()
   {
      var response = ServiceClient.GetCultureName(new GetCultureNameRequest(), CreateLanguageHeader());
      return response.CultureName;
   }
}